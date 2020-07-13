using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories
{
    public interface IJokeRepository : IRepository<Joke, int>
    {
        public void AddUserFavorite(UserFavorite userFavorite);

        public void RemoveUserFavorite(UserFavorite userFavorite);

        public Task<ICollection<UserFavorite>> FindUserFavoriteAsync(Expression<Func<UserFavorite, bool>> filter);

        public Task<ICollection<JokeDto>> FindUserFavoritesJokesTopAsync(string userId, int topNumber);

        public Expression<Func<Joke, JokeDto>> JokeToJokeDtoSelector();

        public void RemoveJoke(Joke joke);

        public Task UpdateJokeCategoriesAsync(int jokeId, ICollection<int> categories);
    }
}
