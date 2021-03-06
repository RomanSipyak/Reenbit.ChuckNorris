﻿using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IJokeService
    {
        Task<JokeDto> GetRandomJokeAsync(string category);

        Task<ICollection<JokeDto>> SearchJokesAsync(string query);

        Task<ICollection<JokeDto>> GetAllJokesAsync();

        Task<JokeDto> GetJokeAsync(int jokeId);

        Task<JokeDto> CreateNewJokeAsync(CreateJokeDto jokeDto);

        Task AddJokeToFavoriteAsync(int favoriteJokeId, int userId);

        Task<JokeDto> UpdateJokeAsync(UpdateJokeDto jokeDto);

        Task DeleteJokeFromFavoriteAsync(int favoriteJokeId, int userId);

        Task<ICollection<JokeDto>> GetFavoriteJokesForUserAsync(int userId);

        Task<ICollection<JokeDto>> GetTopFavoriteJokesAsync();

        Task DeleteJokeAsync(int jokeId);

        Task<ICollection<JokeDto>> GetTopFavoriteJokesForUserAsync(int userid);
    }
}
