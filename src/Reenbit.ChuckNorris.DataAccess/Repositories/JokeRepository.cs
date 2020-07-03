using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Repositories
{
    public class JokeRepository : EntityFrameworkCoreRepository<Joke, int>, IJokeRepository
    {
        private readonly IMapper mapper;

        public JokeRepository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<JokeDTO> GetJokeDtoByIdWithCategoriesAsync(int id)
        {
            var joke = await this.DBSet.Where(j => j.Id == id).FirstOrDefaultAsync();
            var categoriesForJoke = await this.DbContext.Set<Category>().Where(c => c.JokeCategories.Any(jc => jc.JokeId == id)).ToListAsync();
            var JokeDtoForReturn = this.mapper.Map<JokeDTO>(joke);
            if (categoriesForJoke != null)
            {
                JokeDtoForReturn.Categories = categoriesForJoke.Select(c => c.Title).ToList();
            }

            return JokeDtoForReturn;
        }
    }
}
