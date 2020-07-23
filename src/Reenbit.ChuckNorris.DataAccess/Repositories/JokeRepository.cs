using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<ICollection<JokeDTO>> FindUserFavoritesJokesTopAsync(int userId, int topNumber)
        {
            return await this.DbContext.Set<UserFavorite>().AsQueryable()
                .Where(uf => uf.UserId == userId)
                .OrderByDescending(uf => uf.CreatedAt).Take(topNumber)
                .Select(UserFavoriteToJokeDtoSelector()).ToListAsync();
        }

        public async Task<ICollection<JokeDTO>> GetFavoritesJokesTopAsync(int topNumber)
        {
            var favoriteJokes = await ((from j in this.DbContext.Set<Joke>().Include(j => j.UserFavorites)
                                        orderby j.UserFavorites.Count() descending
                                        select new JokeDTO
                                        {
                                            Id = j.Id,
                                            Value = j.Value,
                                            CreatedAt = j.CreatedAt,
                                            UpdatedAt = j.UpdatedAt,
                                            Categories = j.JokeCategories.Select(jc => jc.Category.Title).ToList(),
                                            ImagesUrls = j.Images.Select(i => i.Value).ToList(),
                                        })).Take(topNumber).ToListAsync();
            return favoriteJokes;
        }

        public async Task<ICollection<JokeDTO>> FindFavoriteJokesForUser(int userId)
        {
            return await this.DbContext.Set<UserFavorite>().AsQueryable()
                .Where(uf => uf.UserId == userId)
                .OrderByDescending(uf => uf.CreatedAt)
                .Select(UserFavoriteToJokeDtoSelector()).ToListAsync();
        }

        public Expression<Func<Joke, JokeDTO>> JokeToJokeDtoSelector()
        {
            return j => new JokeDTO
            {
                Id = j.Id,
                Value = j.Value,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt,
                Categories = j.JokeCategories.Select(jc => jc.Category.Title).ToList(),
                ImagesUrls = j.Images.Select(i => i.Value).ToList()
            };
        }

        private Expression<Func<UserFavorite, JokeDTO>> UserFavoriteToJokeDtoSelector()
        {
            return uf => new JokeDTO
            {
                Id = uf.Joke.Id,
                Value = uf.Joke.Value,
                CreatedAt = uf.Joke.CreatedAt,
                UpdatedAt = uf.Joke.UpdatedAt,
                Categories = uf.Joke.JokeCategories.Select(jc => jc.Category.Title).ToList(),
                ImagesUrls = uf.Joke.Images.Select(i => i.Value).ToList()
            };
        }

        public void RemoveLinkedJokeCategories(ICollection<JokeCategory> jokeCategories)
        {
            this.DbContext.Set<JokeCategory>().RemoveRange(jokeCategories);
        }

        public void RemoveLinkedUserFavorites(ICollection<UserFavorite> userFavorites)
        {
            this.DbContext.Set<UserFavorite>().RemoveRange(userFavorites);
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
        }
    }
}