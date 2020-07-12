using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IJokeService
    {
        Task<JokeDto> GetRandomJokeAsync(string category);

        Task<ICollection<JokeDto>> SearchJokesAsync(string query);

        Task<ICollection<string>> GetAllCategoriesAsync();

        Task<JokeDto> CreateNewJokeAsync(CreateJokeDto jokeDto);

        Task AddJokeToFavoriteAsync(int favoriteJokeId, int userId);

        Task DeleteJokeFromFavoriteAsync(int favoriteJokeId, int userId);

        Task<ICollection<JokeDto>> GetFavoriteJokesForUserAsync(int userId);

        Task<ICollection<JokeDto>> GetTopFavoriteJokesForUserAsync(int userid);
    }
}
