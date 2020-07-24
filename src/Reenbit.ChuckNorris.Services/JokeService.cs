using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.ConfigClasses;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services
{
    public class JokeService : IJokeService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IMapper mapper;

        private readonly UserManager<User> userManager;

        private readonly IMediaService mediaService;

        private static readonly Random random = new Random();

        private const int MaxQueryLength = 120;

        private const int MinQueryLength = 3;

        private const int TopFiveFavoriteJokes = 5;

        private const int TopThreeFavoriteJokes = 3;

        private readonly IOptions<AzureStorageBlobOptions> azureStorageBlobOptions;

        public JokeService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper, UserManager<User> userManager, IMediaService mediaService, IOptions<AzureStorageBlobOptions> azureStorageBlobOptions)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.mapper = mapper;
            this.userManager = userManager;
            this.mediaService = mediaService;
            this.azureStorageBlobOptions = azureStorageBlobOptions;
        }

        public async Task<JokeDTO> GetRandomJokeAsync(string category)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();

                int? jokeId = await GetRandomJokeId(category, jokeRepository, uow);

                if (jokeId != null)
                {
                    var joke = await jokeRepository.FindAndMapAsync(jokeRepository.JokeToJokeDtoSelector(), j => j.Id == jokeId.Value);
                    return joke.FirstOrDefault();
                }

                return null;
            }
        }

        private async Task<int?> GetRandomJokeId(string category, IJokeRepository jokeRepository, IUnitOfWork uow)
        {
            var categoryRepository = uow.GetRepository<ICategoryRepository>();
            ICollection<int> jokesIds = null;
            if (!string.IsNullOrEmpty(category))
            {
                if (!await categoryRepository.AnyAsync(c => c.Title.Equals(category)))
                {
                    throw new ArgumentException($"No jokes for category \"{category}\" found.");
                }
                else
                {
                    jokesIds = await jokeRepository.FindAndMapAsync(j => j.Id,
                                                                    j => j.JokeCategories.Any(jc => jc.Category.Title.Equals(category)));
                }
            }
            else
            {
                jokesIds = await jokeRepository.FindAndMapAsync(j => j.Id);
            }

            int? randomId = null;
            if (jokesIds != null && jokesIds.Count() != 0)
            {
                randomId = GetRandomElement(jokesIds);
            }

            return randomId;
        }

        public async Task<ICollection<JokeDTO>> SearchJokesAsync(string query)
        {
            query = query?.Trim();
            if (string.IsNullOrWhiteSpace(query) || query.Length < MinQueryLength || query.Length > MaxQueryLength)
            {
                throw new ArgumentException("search.query: size must be between 3 and 120");
            }

            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                ICollection<JokeDTO> returnJokesDtos = await jokeRepository.FindAndMapAsync(jokeRepository.JokeToJokeDtoSelector(), j => j.Value.Contains(query));
                return returnJokesDtos;
            }
        }

        public async Task<ICollection<JokeDTO>> GetAllJokesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                ICollection<JokeDTO> returnJokesDtos = await jokeRepository.FindAndMapAsync(jokeRepository.JokeToJokeDtoSelector());
                return returnJokesDtos;
            }
        }

        public async Task<JokeDTO> GetJokeAsync(int jokeId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                JokeDTO returnJokeDto = await jokeRepository.FindByKeyAndMapAsync(j => j.Id == jokeId, jokeRepository.JokeToJokeDtoSelector());
                return returnJokeDto;
            }
        }

        public async Task<JokeDTO> CreateNewJokeAsync(CreateJokeDto jokeDto)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                Joke joke = mapper.Map<Joke>(jokeDto);
                var categories = categoryRepository.Find(c => jokeDto.Categories.Any(cd => cd == c.Id));
                jokeRepository.Add(joke);
                joke.JokeCategories = categories.Select(c => new JokeCategory() { Category = c, Joke = joke }).ToList();
                await uow.SaveChangesAsync();
                await AddImagesToJoke(jokeDto, joke);
                await uow.SaveChangesAsync();
                var returnJoke = mapper.Map<JokeDTO>(joke);
                returnJoke.Categories = joke.JokeCategories.Select(jc => jc.Category.Title).ToList();
                returnJoke.ImageUrls = joke.JokeImages.Select(i => i.Url).ToList();
                return returnJoke;
            }
        }

        private async Task AddImagesToJoke(CreateJokeDto jokeDto, Joke joke)
        {
            if (jokeDto.ImageNames.Count() != 0)
            {
                foreach (var imageName in jokeDto.ImageNames)
                {
                    if (string.IsNullOrWhiteSpace(imageName))
                    {
                        continue;
                    }

                    var imageUrl = await mediaService.CopyFile(imageName,
                                                               $"{joke.Id}-{imageName}",
                                                               this.azureStorageBlobOptions.Value.FileTempPath,
                                                               this.azureStorageBlobOptions.Value.FilePath);
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        JokeImage image = new JokeImage
                        {
                            JokeId = joke.Id,
                            Url = imageUrl
                        };

                        joke.JokeImages.Add(image);
                    }
                }
            }
        }

        public async Task DeleteJokeAsync(int jokeId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var joke = (await jokeRepository.FindAndMapAsync(j => j,
                                                                 j => j.Id == jokeId,
                                                                 null,
                                                                 new List<Expression<Func<Joke, object>>> { j => j.JokeCategories, j => j.UserFavorites }))
                                                                 .FirstOrDefault();
                if (joke != null)
                {
                    if (joke.JokeCategories.Count() != 0)
                    {
                        jokeRepository.RemoveLinkedJokeCategories(joke.JokeCategories);
                    }

                    if (joke.UserFavorites.Count() != 0)
                    {
                        jokeRepository.RemoveLinkedUserFavorites(joke.UserFavorites);
                    }

                    jokeRepository.Remove(joke);
                }

                await uow.SaveChangesAsync();
            }
        }

        public async Task AddJokeToFavoriteAsync(int favoriteJokeId, int userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                if (!await jokeRepository.AnyAsync(j => j.Id == favoriteJokeId))
                {
                    throw new ArgumentException($"Joke with Id = {favoriteJokeId} doesn't exist");
                }

                if (await jokeRepository.AnyAsync(j => j.Id == favoriteJokeId && j.UserFavorites.Any(uf => uf.UserId == userId)))
                {
                    throw new ArgumentException($"Joke with Id = {favoriteJokeId} already your favorite");
                }

                jokeRepository.AddUserFavorite(new UserFavorite { JokeId = favoriteJokeId, UserId = userId, CreatedAt = DateTime.UtcNow });
                await uow.SaveChangesAsync();
            }
        }

        public async Task DeleteJokeFromFavoriteAsync(int favoriteJokeId, int userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokeForDelete = (await jokeRepository.FindUserFavoriteAsync(uf => uf.UserId == userId && uf.JokeId == favoriteJokeId));
                if (favoriteJokeForDelete != null)
                {
                    jokeRepository.RemoveUserFavorite(favoriteJokeForDelete);
                }

                await uow.SaveChangesAsync();
            }
        }

        public async Task<ICollection<JokeDTO>> GetFavoriteJokesForUserAsync(int userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.FindFavoriteJokesForUser(userId);
                return favoriteJokes;
            }
        }

        public async Task<ICollection<JokeDTO>> GetTopFavoriteJokesForUserAsync(int userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.FindUserFavoritesJokesTopAsync(userId, TopThreeFavoriteJokes);
                return favoriteJokes;
            }
        }

        public async Task<ICollection<JokeDTO>> GetTopFavoriteJokesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.GetFavoritesJokesTopAsync(TopFiveFavoriteJokes);
                return favoriteJokes;
            }
        }

        public async Task<JokeDTO> UpdateJokeAsync(UpdateJokeDto jokeDto)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var joke = (await jokeRepository.FindAndMapAsync(j => j,
                                                                j => j.Id == jokeDto.Id,
                                                                null,
                                                                new List<Expression<Func<Joke, object>>> { j => j.JokeCategories }))
                                                                .FirstOrDefault();
                if (joke == null)
                {
                    throw new ArgumentException($"Joke with Id = {jokeDto.Id} does't exist");
                }

                joke.Value = jokeDto.Value;
                await jokeRepository.UpdateJokeCategoriesAsync(joke, jokeDto.Categories);
                jokeRepository.Update(joke);
                await uow.SaveChangesAsync();
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                JokeDTO returnJokeDto = this.mapper.Map<JokeDTO>(joke);
                returnJokeDto.Categories = (await categoryRepository.FindAndMapAsync(c => c.Title, c => c.JokeCategories.Any(jc => jc.JokeId == joke.Id))).ToList();
                return returnJokeDto;
            }
        }

        private T GetRandomElement<T>(IEnumerable<T> collection)
        {
            return collection.ElementAt(random.Next(collection.Count()));
        }
    }
}
