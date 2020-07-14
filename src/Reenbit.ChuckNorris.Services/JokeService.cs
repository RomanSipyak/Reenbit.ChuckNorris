using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services
{
    public class JokeService : IJokeService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IMapper mapper;

        private readonly UserManager<User> userManager;

        private static readonly Random random = new Random();

        private const int MaxQueryLength = 120;

        private const int MinQueryLength = 3;

        public JokeService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper, UserManager<User> userManager)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public async Task<JokeDto> GetRandomJokeAsync(string category)
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

        public async Task<ICollection<JokeDto>> SearchJokesAsync(string query)
        {
            query = query?.Trim();
            if (string.IsNullOrWhiteSpace(query) || query.Length < MinQueryLength || query.Length > MaxQueryLength)
            {
                throw new ArgumentException("search.query: size must be between 3 and 120");
            }

            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                ICollection<JokeDto> returnJokesDtos = await jokeRepository.FindAndMapAsync(jokeRepository.JokeToJokeDtoSelector(), j => j.Value.Contains(query));
                return returnJokesDtos;
            }
        }

        public async Task<ICollection<JokeDto>> GetAllJokesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                ICollection<JokeDto> returnJokesDtos = await jokeRepository.FindAndMapAsync(jokeRepository.JokeToJokeDtoSelector());
                return returnJokesDtos;
            }
        }

        public async Task<JokeDto> GetJokeAsync(int jokeId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                JokeDto returnJokeDto = await jokeRepository.FindByKeyAndMapAsync(j => j.Id == jokeId, jokeRepository.JokeToJokeDtoSelector());
                return returnJokeDto;
            }
        }

        public async Task<JokeDto> CreateNewJokeAsync(CreateJokeDto jokeDto)
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
                var returnJoke = mapper.Map<JokeDto>(joke);
                returnJoke.Categories = joke.JokeCategories.Select(jc => jc.Category.Title).ToList();
                return returnJoke;
            }
        }

        public async Task DeleteJokeAsync(int jokeId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var joke = await jokeRepository.GetByIdAsync(jokeId);
                jokeRepository.RemoveJoke(joke);
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

        public async Task<ICollection<JokeDto>> GetFavoriteJokesForUserAsync(int userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.FindFavoriteJokesForUser(userId);
                return favoriteJokes;
            }
        }

        public async Task<ICollection<JokeDto>> GetTopFavoriteJokesForUserAsync(int userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.FindUserFavoritesJokesTopAsync(userId, 3);
                return favoriteJokes;
            }
        }

        public async Task<ICollection<JokeDto>> GetTopFavoriteJokesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.GetFavoritesJokesTopAsync(5);
                return favoriteJokes;
            }
        }

        public async Task<JokeDto> UpdateJokeAsync(UpdateJokeDto jokeDto)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                if (!await jokeRepository.AnyAsync(j => j.Id == jokeDto.Id))
                {
                    throw new ArgumentException($"Joke with Id = {jokeDto.Id} does't exist");
                }

                var joke = this.mapper.Map<Joke>(jokeDto);
                joke.CreatedAt = await jokeRepository.FindByKeyAndMapAsync(j => j.Id == joke.Id, j => j.CreatedAt);
                await jokeRepository.UpdateJokeCategoriesAsync(joke.Id, jokeDto.Categories);
                jokeRepository.Update(joke);
                var number = await uow.SaveChangesAsync();
                JokeDto returnJokeDto = await jokeRepository.FindByKeyAndMapAsync(j => j.Id == jokeDto.Id, jokeRepository.JokeToJokeDtoSelector());
                return returnJokeDto;
            }
        }

        private T GetRandomElement<T>(IEnumerable<T> collection)
        {
            return collection.ElementAt(random.Next(collection.Count()));
        }
    }
}
