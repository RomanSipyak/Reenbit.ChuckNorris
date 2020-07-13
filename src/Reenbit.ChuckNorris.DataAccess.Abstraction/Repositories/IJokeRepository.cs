using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface IJokeRepository : IRepository<Joke, int>
    {
        public void AddUserFavorite(UserFavorite userFavorite);

        public void RemoveUserFavorite(UserFavorite userFavorite);

        public Task<UserFavorite> FindUserFavoriteAsync(Expression<Func<UserFavorite, bool>> filter);

        public Task<ICollection<JokeDto>> FindUserFavoritesJokesTopAsync(int userId, int topNumber);

        public Expression<Func<Joke, JokeDto>> JokeToJokeDtoSelector();

        Task<ICollection<JokeDto>> FindFavoriteJokesForUser(int userId);
    }
}
