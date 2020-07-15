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

        public async Task<ICollection<JokeDto>> FindUserFavoritesJokesTopAsync(int userId, int topNumber)
        {
            return await this.DbContext.Set<UserFavorite>().AsQueryable()
                .Where(uf => uf.UserId == userId)
                .OrderByDescending(uf => uf.CreatedAt).Take(topNumber)
                .Select(UserFavoriteToJokeDtoSelector()).ToListAsync();
        }

        public async Task<ICollection<JokeDto>> GetFavoritesJokesTopAsync(int topNumber)
        {
            /* var userFavoriteIds = await this.DbContext.Set<UserFavorite>().AsQueryable()
                                                                .GroupBy(uf => uf.JokeId)
                                                                .OrderByDescending(g => g.Count())
                                                                .Take(topNumber)
                                                                .Select(g => g.Key)
                                                                .Join(this.DbContext.Set<Joke>(),
                                                                 JokeId => JokeId,
                                                                 j => j.,
                                                                 JokeToJokeDtoSelector())
                                                                .ToListAsync();
             var favoriteJokes = this.DbContext.Set<Joke>().AsQueryable()
                                                    .Where(j => userFavoriteIds.Contains(j.Id))
                                                    .Select(JokeToJokeDtoSelector()).ToList()
                                                    .OrderBy(jd => userFavoriteIds.IndexOf(jd.Id))
                                                    .ToList();
             return favoriteJokes;*/

            /*  var favoriteJokes = await ((from uf in this.DbContext.Set<UserFavorite>()
                                         group uf by uf.JokeId into g
                                         orderby g.Count() descending
                                         join j in this.DBSet on g.Key equals j.Id
                                         select new JokeDto
                                         {
                                             Id = j.Id,
                                             IconUrl = j.IconUrl,
                                             Url = j.Url,
                                             Value = j.Value,
                                             CreatedAt = j.CreatedAt,
                                             UpdatedAt = j.UpdatedAt,
                                             Categories = j.JokeCategories.Select(jc => jc.Category.Title).ToList()
                                         }).Take(5)).ToListAsync();*/

            /*  var favoriteJokes = ((from uf in this.DbContext.Set<UserFavorite>().Include(uf => uf.Joke)
                                    group uf.Joke by uf.JokeId into g
                                    orderby g.Count() descending)).Take(5).ToArray();*/

            /*   var results = this.DbContext.Set<UserFavorite>().Include(uf => uf.Joke).GroupBy(z => z.JokeId, z => z.Joke).Select(g => new
               {
                   Aggregate = g.First()
               }).ToArray();*/
         
            var favoriteJokes = ((from j in this.DbContext.Set<Joke>().Include( j => j.UserFavorites)
                                       orderby j.UserFavorites.Count() descending
                                       select new JokeDto
                                       {
                                           Id = j.Id,
                                           IconUrl = j.IconUrl,
                                           Url = j.Url,
                                           Value = j.Value,
                                           CreatedAt = j.CreatedAt,
                                           UpdatedAt = j.UpdatedAt,
                                           Categories = j.JokeCategories.Select(jc => jc.Category.Title).ToList(),
                                       })).Take(5).ToSql();
            return null;
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

    public static class QueryableExtensions
    {
        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            string sql = command.CommandText;
            return sql;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }
}