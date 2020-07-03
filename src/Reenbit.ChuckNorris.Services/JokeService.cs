using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs;
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
                if (!await categoryRepository.AnyAsync(ct => ct.Title.Equals(category)))
                {
                    throw new ArgumentException($"No jokes for category \"{category}\" found.");
                }
                else
                {
                    jokesIds = await jokeRepository.FindAndMapAsync(jk => jk.Id,
                                                            jk => jk.JokeCategories.Any(jck => jck.Category.Title.Equals(category)));
                }
            }
            else
            {
                jokesIds = await jokeRepository.FindAndMapAsync(jk => jk.Id);
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
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                ICollection<int> jokesIds = null;
                ICollection<JokeDTO> returnJokesDtos = new List<JokeDTO>();

                if (string.IsNullOrWhiteSpace(query) || query.Length < 3 || query.Length > 120)
                {
                    throw new ArgumentException("search.query: size must be between 3 and 120");
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

        public async Task<ICollection<string>> GetAllCategoriesAsync()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
            {
                var categoryRepository = uow.GetRepository<ICategoryRepository>();

                return await categoryRepository.FindAndMapAsync(ct => ct.Title, null, null, null);
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
