using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt,
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
                CreatedAt = uf.Joke.CreatedAt,
                UpdatedAt = uf.Joke.UpdatedAt,
                Categories = uf.Joke.JokeCategories.Select(jc => jc.Category.Title).ToList()
            };
        }

        public void RemoveJoke(Joke joke)
        {
            if (joke != null)
            {
                RemoveLinkedJokeCategories(joke);
                RemoveLinkedUserFavorites(joke);
                this.Remove(joke);
            }
        }

        private void RemoveLinkedJokeCategories(Joke joke)
        {
            var jokeCategories = this.DbContext.Set<JokeCategory>().AsQueryable().Where(jc => jc.JokeId == joke.Id);
            if (jokeCategories.Count() != 0)
            {
                this.DbContext.Set<JokeCategory>().RemoveRange(jokeCategories);
            }
        }

        private void RemoveLinkedUserFavorites(Joke joke)
        {
            var userFavorites = this.DbContext.Set<UserFavorite>().AsQueryable().Where(jc => jc.JokeId == joke.Id);
            if (userFavorites.Count() != 0)
            {
                this.DbContext.Set<UserFavorite>().RemoveRange(userFavorites);
            }
        }

        public async Task UpdateJokeCategoriesAsync(int jokeId, ICollection<int> categories)
        {
            var intersectJokesKategories = await this.DbContext.Set<JokeCategory>().AsQueryable()
                .Where(jc => jc.JokeId == jokeId && categories.Any(c => c == jc.CategoryId)).ToListAsync();
            var leftSetJokeCategories = await this.DbContext.Set<JokeCategory>().AsQueryable()
                .Where(jc => jc.JokeId == jokeId && categories.All(c => c != jc.CategoryId)).ToListAsync();
            var rightSetJokeCategoriesIds = categories
                .Except(intersectJokesKategories.Select(jc => jc.CategoryId))
                .Except(leftSetJokeCategories.Select(jc => jc.CategoryId))
                .Select(c => new JokeCategory { CategoryId = c, JokeId = jokeId}).ToList();
            this.DbContext.Set<JokeCategory>().RemoveRange(leftSetJokeCategories);
            this.DbContext.Set<JokeCategory>().AddRange(rightSetJokeCategoriesIds);
            intersectJokesKategories.AddRange(rightSetJokeCategoriesIds);
        }
    }
}