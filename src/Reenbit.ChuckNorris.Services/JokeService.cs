using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
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

        public async Task<ICollection<string>> GetAllCategoriesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var categoryRepository = uow.GetRepository<ICategoryRepository>();

                return await categoryRepository.FindAndMapAsync(c => c.Title);
            }
        }

        public async Task<bool> AddJokeToFavoriteAsync(int favoriteJokeId, string userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                if (!await jokeRepository.AnyAsync(j => j.Id == favoriteJokeId))
                {
                    throw new ArgumentException($"Joke with Id = {favoriteJokeId} doesn't exist");
                }

                if (await jokeRepository.AnyAsync(j => j.Id == favoriteJokeId && j.UserFavorites.Any(uf => uf.UserId == Int32.Parse(userId))))
                {
                    throw new ArgumentException($"Joke with Id = {favoriteJokeId} already your favorite");
                }

                jokeRepository.AddUserFavorite(new UserFavorite { JokeId = favoriteJokeId, UserId = Int32.Parse(userId), CreatedAt = DateTime.UtcNow });
                var number = await (uow.SaveChangesAsync());
                return number > 0;
            }
        }

        public async Task<bool> DeleteJokeFromFavoriteAsync(int favoriteJokeId, string userId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokeForDelete = (await jokeRepository.FindUserFavoriteAsync(uf => uf.UserId == Int32.Parse(userId) && uf.JokeId == favoriteJokeId)).FirstOrDefault();
                if (favoriteJokeForDelete != null)
                {
                    jokeRepository.RemoveUserFavorite(favoriteJokeForDelete);
                }

                var number = await (uow.SaveChangesAsync());
                return number > 0;
            }
        }

        public async Task<ICollection<JokeDto>> GetFavoriteJokesForUserAsync(string userid)
        {
            var user = await this.userManager.FindByIdAsync(userid);
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.FindAndMapAsync(jokeRepository.JokeToJokeDtoSelector(), j => j.UserFavorites.Any(uf => uf.UserId == user.Id));
                return favoriteJokes;
            }
        }

        public async Task<ICollection<JokeDto>> GetTopFavoriteJokesForUserAsync(string userid)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.FindUserFavoritesJokesTopAsync(userid, 3);
                return favoriteJokes;
            }
        }

        private T GetRandomElement<T>(IEnumerable<T> collection)
        {
            return collection.ElementAt(random.Next(collection.Count()));
        }
    }
}
