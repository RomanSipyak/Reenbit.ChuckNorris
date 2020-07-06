using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services.Abstraction;
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

                int? jokeId = await GetRandomJokeId(category, jokeRepository, uow);

                if (jokeId != null)
                {
                    var joke = await jokeRepository.FindAndMapAsync(j =>
                                                   new JokeDTO
                                                   {
                                                       Id = j.Id,
                                                       IconUrl = j.IconUrl,
                                                       Url = j.Url,
                                                       Value = j.Value,
                                                       Categories = j.JokeCategories.Select(jc => jc.Category.Title).ToList()
                                                   },
                                                   j => j.Id == jokeId.Value);
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
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3 || query.Length > 120)
            {
                throw new ArgumentException("search.query: size must be between 3 and 120");
            }

            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                ICollection<JokeDTO> returnJokesDtos = await jokeRepository.FindAndMapAsync(j =>
                                                       new JokeDTO
                                                       {
                                                           Id = j.Id,
                                                           IconUrl = j.IconUrl,
                                                           Url = j.Url,
                                                           Value = j.Value,
                                                           Categories = j.JokeCategories.Select(jc => jc.Category.Title).ToList()
                                                       },
                                                       j => j.Value.Contains(query));
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
                var categories = categoryRepository.Find(c => jokeDto.Categories.Any(cd => cd == c.Id));
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

        public async Task<ICollection<string>> GetAllCategoriesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var categoryRepository = uow.GetRepository<ICategoryRepository>();

                return await categoryRepository.FindAndMapAsync(c => c.Title);
            }
        }

        private T GetRandomElement<T>(IEnumerable<T> collection)
        {
            return collection.ElementAt(random.Next(collection.Count()));
        }
    }
}
