using AutoMapper;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services
{
    public class JokeService : IJokeService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IMapper mapper;
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public JokeService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.mapper = mapper;
        }

        public async Task<JokeDTO> GetRundomJokeAsync(IEnumerable<string> categories)
        {
           using(IUnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork())
           {
                var jokeRepository = uow.GetRepository<IJokeRepository>();
                var categoryRepository = uow.GetRepository<ICategoryRepository>();
                ICollection<int> jokesIds;

                if (categories != null && categories.Any())
                {
                    string unexistedCategory = null;

                    foreach(var category in categories)
                    {
                        if(!await categoryRepository.CategoryExistAsync(ct => ct.Title.Equals(category)))
                        {
                            unexistedCategory = category;
                            break;
                        }
                    }

                    if(unexistedCategory != null)
                    {
                        throw new ArgumentException($"No jokes for category \"{unexistedCategory}\" found.");
                    }
                    else
                    {
                        jokesIds = await jokeRepository.FindAndMapAsync(jk => jk.Id,
                                                                        jk => jk.JokeCategories.Any(jck => categories.Any(ct => jck.Category.Title.Equals(ct))));
                    }
                }
                else
                {
                    jokesIds = await jokeRepository.FindAndMapAsync(jk => jk.Id);
                }

                int? randomId = null;
                if(jokesIds.Count() != 0)
                {
                    randomId = GetRundomeElement(jokesIds);
                }

                if (randomId != null)
                {
                    var joke = await jokeRepository.GetByIdAsync(randomId.Value);
                    return this.mapper.Map<JokeDTO>(joke);
                }
                return null;
           }
        }

        private static int RandomNumber(int number)
        {
            lock (syncLock)
            {
                return random.Next(number);
            }
        }

        private T GetRundomeElement<T>(IEnumerable<T> collection)
        {
            return collection.ElementAt(RandomNumber(collection.Count()));
        }
    }
}
