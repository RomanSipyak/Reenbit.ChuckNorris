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
        void AddUserFavorite(UserFavorite userFavorite);

        void RemoveUserFavorite(UserFavorite userFavorite);

        Task<UserFavorite> FindUserFavoriteAsync(Expression<Func<UserFavorite, bool>> filter);

        Task<ICollection<JokeDto>> FindUserFavoritesJokesTopAsync(int userId, int topNumber);

        Task<ICollection<JokeDto>> GetFavoritesJokesTopAsync(int topNumber);

        Expression<Func<Joke, JokeDto>> JokeToJokeDtoSelector();

        void RemoveLinkedJokeCategories(ICollection<JokeCategory> jokeCategories);

        void RemoveLinkedUserFavorites(ICollection<UserFavorite> userFavorites);

        void RemoveLinkedImages(ICollection<JokeImage> jokeImages);

        Task UpdateJokeCategoriesAsync(Joke joke, ICollection<int> categories);

        Task<ICollection<JokeDto>> FindFavoriteJokesForUser(int userId);
    }
}
