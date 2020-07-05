using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services.Abstraction;
using Reenbit.ChuckNorris.Services.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services
{
    public class JokeService : IJokeService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IMapper mapper;

        private static readonly Random random = new Random();

        public JokeService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.mapper = mapper;
        }

        public async Task<JokeDTO> GetRandomJokeAsync(string category)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                int? jokeId = null;

                jokeId = await GetRandomJokeId(category, jokeRepository, categoryRepository);

                if (jokeId != null)
                {
                    var joke = await jokeRepository.GetJokeDtoByIdWithCategoriesAsync(jokeId.Value);
                    return joke;
                }

                return null;
            }
        }

        private async Task<int?> GetRandomJokeId(string category, IJokeRepository jokeRepository, ICategoryRepository categoryRepository)
        {
            ICollection<int> jokesIds = null;
            if (!string.IsNullOrEmpty(category))
            {
                if (!await categoryRepository.AnyAsync(c => c.Title.Equals(category)))
                {
                    throw new CategoryNotFoundException($"No jokes for category \"{category}\" found.");
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

        public async Task<ICollection<JokeDTO>> GetJokesBySearch(string query)
        {
            query = query?.Trim();
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                ICollection<int> jokesIds = null;
                ICollection<JokeDTO> returnJokesDtos = new List<JokeDTO>();

                if (string.IsNullOrWhiteSpace(query) || query.Length < 3 || query.Length > 120)
                {
                    throw new SearchQueryException("search.query: size must be between 3 and 120");
                }
                else
                {
                    jokesIds = await jokeRepository.FindAndMapAsync(j => j.Id, j => j.Value.Contains(query));
                }

                if (jokesIds != null && jokesIds.Count() != 0)
                {
                    foreach (int id in jokesIds)
                    {
                        var jokeDto = await jokeRepository.GetJokeDtoByIdWithCategoriesAsync(id);
                        returnJokesDtos.Add(jokeDto);
                    }
                }

                return returnJokesDtos;
            }
        }

        public async Task<JokeDTO> CreateNewJokeAsync(CreateJokeDTO jokeDto)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                Joke joke = mapper.Map<Joke>(jokeDto);
                var categories = mapper.Map<ICollection<Category>>(jokeDto.Categories);
                await AllCategoriesExistAsync(categories, categoryRepository);
                jokeRepository.Add(joke);

                foreach (var category in categories)
                {
                    var categoryForJoke = (await categoryRepository.FindAsync(c => category.Id == c.Id)).FirstOrDefault();
                    joke.JokeCategories?.Add(new JokeCategory() { Category = categoryForJoke, Joke = joke });
                }

                await uow.SaveChangesAsync();
                var returnJoke = mapper.Map<JokeDTO>(joke);
                returnJoke.Categories = joke.JokeCategories.Select(jc => jc.Category.Title).ToList();
                return returnJoke;
            }
        }

        private async Task<bool> AllCategoriesExistAsync(ICollection<Category> categories, ICategoryRepository categoryRepository)
        {
            foreach (var category in categories)
            {
                if (!await categoryRepository.AnyAsync(c => category.Id == c.Id))
                {
                    throw new CategoryNotFoundException($"Category with Id = {category.Id} not found.");
                }
            }
            return true;
        }

        public async Task<ICollection<string>> GetAllCategoriesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var categoryRepository = uow.GetRepository<ICategoryRepository>();

                return await categoryRepository.FindAndMapAsync(c => c.Title, null, null, null);
            }
        }

        private static int RandomNumber(int number)
        {
            return random.Next(number);
        }

        private T GetRandomElement<T>(IEnumerable<T> collection)
        {
            return collection.ElementAt(RandomNumber(collection.Count()));
        }
    }
}
