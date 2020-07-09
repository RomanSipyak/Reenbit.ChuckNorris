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
        public void AddUserFavorite(UserFavorite userFavorite)
        {
            this.DbContext.Set<UserFavorite>().Add(userFavorite);
        }

        public void RemoveUserFavorite(UserFavorite userFavorite)
        {
            this.DbContext.Set<UserFavorite>().Remove(userFavorite);
        }

        public async Task<ICollection<UserFavorite>> FindUserFavoriteAsync(Expression<Func<UserFavorite, bool>> filter)
        {
            return await this.DbContext.Set<UserFavorite>().AsQueryable().Where(filter).ToListAsync();
        }

        public async Task<ICollection<JokeDto>> FindUserFavoritesJokesTopAsync(string userId, int topNumber)
        {
            return await this.DbContext.Set<UserFavorite>().AsQueryable().Where(uf => uf.UserId == Int32.Parse(userId)).
                                                      OrderByDescending(uf => uf.CreatedAt).Take(topNumber)
                                                      .Select(UserFavoriteToJokeDtoSelector()).ToListAsync();
        }

        public Expression<Func<Joke, JokeDto>> JokeToJokeDtoSelector()
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

        private Expression<Func<UserFavorite, JokeDto>> UserFavoriteToJokeDtoSelector()
        {
            return uf => new JokeDto
            {
                Id = uf.Joke.Id,
                IconUrl = uf.Joke.IconUrl,
                Url = uf.Joke.Url,
                Value = uf.Joke.Value,
                Categories = uf.Joke.JokeCategories.Select(jc => jc.Category.Title).ToList()
            };
        }
    }
}
