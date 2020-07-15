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

        public async Task<UserFavorite> FindUserFavoriteAsync(Expression<Func<UserFavorite, bool>> filter)
        {
            return await this.DbContext.Set<UserFavorite>().AsQueryable().Where(filter).FirstAsync();
        }

        public async Task<ICollection<JokeDto>> FindUserFavoritesJokesTopAsync(int userId, int topNumber)
        {
            return await this.DbContext.Set<UserFavorite>().AsQueryable()
                .Where(uf => uf.UserId == userId)
                .OrderByDescending(uf => uf.CreatedAt).Take(topNumber)
                .Select(UserFavoriteToJokeDtoSelector()).ToListAsync();
        }

        public async Task<ICollection<JokeDto>> GetFavoritesJokesTopAsync(int topNumber)
        {
            var list = await this.DbContext.Set<UserFavorite>().AsQueryable()
                                                               .GroupBy(uf => uf.JokeId)
                                                               .OrderByDescending(g => g.Count())
                                                               .Take(topNumber)
                                                               .Select(g => g.Key)
                                                               .ToListAsync();
            var result = this.DbContext.Set<Joke>().AsQueryable()
                                                   .Where(j => list.Contains(j.Id))
                                                   .Select(JokeToJokeDtoSelector()).ToList()
                                                   .OrderBy(jd => list.IndexOf(jd.Id))
                                                   .ToList();
            return result;
        }

        public async Task<ICollection<JokeDto>> FindFavoriteJokesForUser(int userId)
        {
            return await this.DbContext.Set<UserFavorite>().AsQueryable()
                .Where(uf => uf.UserId == userId)
                .OrderByDescending(uf => uf.CreatedAt)
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

        /* public void RemoveJoke(Joke joke)
         {
             if (joke != null)
             {
               *//*  RemoveLinkedJokeCategories(joke);
                 RemoveLinkedUserFavorites(joke);*//*
                 this.Remove(joke);
             }
         }*/

        public void RemoveLinkedJokeCategories(ICollection<JokeCategory> jokeCategories)
        {
            /* var jokeCategories = this.DbContext.Set<JokeCategory>().AsQueryable().Where(jc => jc.JokeId == joke.Id);
             if (jokeCategories.Count() != 0)
             {*/
            this.DbContext.Set<JokeCategory>().RemoveRange(jokeCategories);
            /* }*/
        }

        public void RemoveLinkedUserFavorites(ICollection<UserFavorite> userFavorites)
        {
            /* var userFavorites = this.DbContext.Set<UserFavorite>().AsQueryable().Where(jc => jc.JokeId == joke.Id);
             if (userFavorites.Count() != 0)
             {*/
            this.DbContext.Set<UserFavorite>().RemoveRange(userFavorites);
            /* }*/
        }

        public async Task UpdateJokeCategoriesAsync(int jokeId, ICollection<int> categories)
        {
            var commonJokesCategories = await this.DbContext.Set<JokeCategory>().AsQueryable()
                .Where(jc => jc.JokeId == jokeId && categories.Any(c => c == jc.CategoryId)).ToListAsync();
            var jokeCategoriesForDelete = await this.DbContext.Set<JokeCategory>().AsQueryable()
                .Where(jc => jc.JokeId == jokeId && categories.All(c => c != jc.CategoryId)).ToListAsync();
            var jokeCategoriesForCreate = categories
                .Except(commonJokesCategories.Select(jc => jc.CategoryId))
                .Except(jokeCategoriesForDelete.Select(jc => jc.CategoryId));
            var rightSetJokeCategories = await this.DbContext.Set<Category>().AsQueryable()
                .Where(c => jokeCategoriesForCreate.Any(ci => ci == c.Id))
                .Select(c => new JokeCategory { CategoryId = c.Id, JokeId = jokeId }).ToListAsync();
            this.DbContext.Set<JokeCategory>().RemoveRange(jokeCategoriesForDelete);
            this.DbContext.Set<JokeCategory>().AddRange(rightSetJokeCategories);
            commonJokesCategories.AddRange(rightSetJokeCategories);
        }

        public async Task UpdateJokeCategoriesAsync(Joke joke, ICollection<int> categories)
        {
            var commonJokesCategories = joke.JokeCategories
                .Where(jc => categories.Any(c => c == jc.CategoryId))
                .ToList();
            var jokeCategoriesForDelete = joke.JokeCategories
                .Where(jc => categories.All(c => c != jc.CategoryId)).ToList();
            var jokeCategoriesIdsForCreate = categories
                .Except(commonJokesCategories.Select(jc => jc.CategoryId))
                .Except(jokeCategoriesForDelete.Select(jc => jc.CategoryId));
            var JokeCategoriesForAdding = await this.DbContext.Set<Category>().AsQueryable()
                .Where(c => jokeCategoriesIdsForCreate.Any(ci => ci == c.Id))
                .Select(c => new JokeCategory { CategoryId = c.Id, JokeId = joke.Id }).ToListAsync();
            joke.JokeCategories = joke.JokeCategories.Except(jokeCategoriesForDelete).Union(JokeCategoriesForAdding).ToList();
           // joke.JokeCategories.ToList().AddRange(JokeCategoriesForAdding);
        }
    }
}