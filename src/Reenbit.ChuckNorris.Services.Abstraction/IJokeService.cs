﻿using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IJokeService
    {
        Task<JokeDto> GetRandomJokeAsync(string category);

        Task<ICollection<JokeDto>> SearchJokesAsync(string query);

        Task<ICollection<JokeDto>> GetAllJokesAsync();

        Task<JokeDto> GetJokeAsync(int jokeId);

        Task<ICollection<string>> GetAllCategoriesAsync();

        Task<JokeDto> CreateNewJokeAsync(CreateJokeDto jokeDto);

        Task<bool> AddJokeToFavoriteAsync(int favoriteJokeId, string userId);

        Task<bool> DeleteJokeFromFavoriteAsync(int favoriteJokeId, string userId);

        Task<ICollection<JokeDto>> GetFavoriteJokesForUserAsync(string userId);

        Task<ICollection<JokeDto>> GetTopFavoriteJokesForUserAsync(string userid);
    }
}
