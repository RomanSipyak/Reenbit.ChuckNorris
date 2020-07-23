using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IJokeService
    {
        Task<JokeDTO> GetRandomJokeAsync(string category);

        Task<ICollection<JokeDTO>> SearchJokesAsync(string query);

        Task<ICollection<JokeDTO>> GetAllJokesAsync();

        Task<JokeDTO> GetJokeAsync(int jokeId);

        Task<JokeDTO> CreateNewJokeAsync(CreateJokeDto jokeDto);

        Task AddJokeToFavoriteAsync(int favoriteJokeId, int userId);

        Task<JokeDTO> UpdateJokeAsync(UpdateJokeDto jokeDto);

        Task DeleteJokeFromFavoriteAsync(int favoriteJokeId, int userId);

        Task<ICollection<JokeDTO>> GetFavoriteJokesForUserAsync(int userId);

        Task<ICollection<JokeDTO>> GetTopFavoriteJokesAsync();

        Task DeleteJokeAsync(int jokeId);

        Task<ICollection<JokeDTO>> GetTopFavoriteJokesForUserAsync(int userid);
    }
}
