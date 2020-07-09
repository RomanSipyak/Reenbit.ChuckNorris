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
                    var joke = await jokeRepository.FindAndMapAsync(JokeDtoSelector(), j => j.Id == jokeId.Value);
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
                ICollection<JokeDto> returnJokesDtos = await jokeRepository.FindAndMapAsync(JokeDtoSelector(), j => j.Value.Contains(query));
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

        public async Task<bool> AddJokeToFavoriteAsync(int favoriteJokeId, ClaimsPrincipal userClaimsPrincipal)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var joke = jokeRepository.Find(j => j.Id == favoriteJokeId, null,
                                               new List<Expression<Func<Joke, object>>>() { j => j.UserFavorites }).FirstOrDefault();
                if (joke == null)
                {
                    throw new ArgumentException($"Joke with Id = {favoriteJokeId} doesn't exist");
                }

                var user = await this.userManager.GetUserAsync(userClaimsPrincipal);
                if (joke.UserFavorites.Any(uf => uf.UserId == user.Id))
                {
                    throw new ArgumentException($"Joke with Id = {favoriteJokeId} already your favorite");
                }

                joke.UserFavorites.Add(new UserFavorite { JokeId = favoriteJokeId, UserId = user.Id });
                var number = await (uow.SaveChangesAsync());
                return number > 0;
            }
        }

        public async Task<bool> DeleteJokeFromFavoriteAsync(int favoriteJokeId, ClaimsPrincipal userClaimsPrincipal)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var joke = jokeRepository.Find(j => j.Id == favoriteJokeId, null,
                                               new List<Expression<Func<Joke, object>>>() { j => j.UserFavorites }).FirstOrDefault();
                if (joke == null)
                {
                    throw new ArgumentException($"Joke with Id = {favoriteJokeId} doesn't exist");
                }

                var user = await this.userManager.GetUserAsync(userClaimsPrincipal);
                if (!joke.UserFavorites.Any(uf => uf.UserId == user.Id))
                {
                    throw new ArgumentException($"Joke with Id = {favoriteJokeId} doesn't your favorite");
                }

                var favoriteJokeForDelete = joke.UserFavorites.Where(uf => uf.UserId == user.Id).FirstOrDefault();
                joke.UserFavorites.Remove(favoriteJokeForDelete);
                var number = await (uow.SaveChangesAsync());
                return number > 0;
            }
        }

        public async Task<ICollection<JokeDto>> GetFavoriteJokesForUser(ClaimsPrincipal userClaimsPrincipal)
        {
            var user = await this.userManager.GetUserAsync(userClaimsPrincipal);
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var favoriteJokes = await jokeRepository.FindAndMapAsync(JokeDtoSelector(), j => j.UserFavorites.Any(uf => uf.UserId == user.Id));
                return favoriteJokes;
            }
        }

        private Expression<Func<Joke, JokeDto>> JokeDtoSelector()
        {
            return j => new JokeDto
            {
                Id = j.Id,
                IconUrl = j.IconUrl,
                Url = j.Url,
                Value = j.Value,
                Categories = j.JokeCategories.Select(jc => jc.Category.Title).ToList()
            };
        }

        private T GetRandomElement<T>(IEnumerable<T> collection)
        {
            return collection.ElementAt(random.Next(collection.Count()));
        }
    }
}
