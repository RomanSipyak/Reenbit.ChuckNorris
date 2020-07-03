using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IJokeService
    {
        Task<JokeDTO> GetRandomJokeAsync(string category);

        Task<ICollection<JokeDTO>> GetJokesBySearch(string query);

        Task<ICollection<string>> GetAllCategoriesAsync();

        Task<JokeDTO> CreateNewJokeAsync(CreateJokeDTO jokeDto);
    }
}
