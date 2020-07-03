using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Controllers
{
    [ApiController]
    [Route("api/jokes")]
    public class JokeControlles : ControllerBase
    {
        private readonly IJokeService jokeService;

        public JokeControlles(IJokeService jokeService)
        {
            this.jokeService = jokeService;
        }

        [HttpGet]
        [Route("random")]
        public async Task<IActionResult> GetRandomJoke([FromQuery] string category)
        {
            var JokeDto = await jokeService.GetRandomJokeAsync(category);
            if (JokeDto == null)
            {
                return BadRequest();
            }

            return Ok(JokeDto);
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> GetJokesBySearch([FromQuery] string query)
        {
            var JokeDtos = await jokeService.GetJokesBySearch(query);
            if (JokeDtos == null)
            {
                return BadRequest();
            }

            return Ok(JokeDtos);
        }

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await jokeService.GetAllCategoriesAsync();
            return Ok(categories);
        }
    }
}
