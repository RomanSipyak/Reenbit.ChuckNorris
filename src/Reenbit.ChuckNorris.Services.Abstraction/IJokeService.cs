using Reenbit.ChuckNorris.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IJokeService
    {
        Task<JokeDTO> GetRundomJokeAsync(IEnumerable<string> categories, string query);
    }
}
