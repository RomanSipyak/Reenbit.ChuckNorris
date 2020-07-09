using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Controllers
{
    [ApiController]
    [Route("api/jokes")]
    public class JokeControlles : BaseApiController
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
            var jokeDto = await jokeService.GetRandomJokeAsync(category);
            if (jokeDto == null)
            {
                return NotFound();
            }

            return Ok(jokeDto);
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> GetJokesBySearch([FromQuery] string query)
        {
            var jokeDtos = await jokeService.SearchJokesAsync(query);
            if (jokeDtos.Count() == 0)
            {
                return NotFound();
            }

            return Ok(jokeDtos);
        }

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await jokeService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJoke(CreateJokeDto createJokeDto)
        {
            var joke = await jokeService.CreateNewJokeAsync(createJokeDto);
            return CreatedAtAction(nameof(CreateJoke), joke);
        }

        [HttpPost]
        [Route("favorite/{favoriteJokeId}")]
        [Authorize]
        public async Task<IActionResult> AddJokeToFavorite([FromRoute] int favoriteJokeId)
        {
            return CreatedAtAction(nameof(AddJokeToFavorite), await jokeService.AddJokeToFavoriteAsync(favoriteJokeId, this.UserId));
        }

        [HttpDelete]
        [Route("favorite/{favoriteJokeId}")]
        [Authorize]
        public async Task<IActionResult> DeleteJokefromFavorite([FromRoute] int favoriteJokeId)
        {
            return Ok(await jokeService.DeleteJokeFromFavoriteAsync(favoriteJokeId, this.UserId));
        }

        [HttpGet]
        [Route("user-favorite")]
        [Authorize]
        public async Task<IActionResult> GetAllFavoriteJokesForUser()
        {
            var userFavoriteJokes = await this.jokeService.GetFavoriteJokesForUserAsync(this.UserId);
            return Ok(userFavoriteJokes);
        }

        [HttpGet]
        [Route("user-favorite-newest-top")]
        [Authorize]
        public async Task<IActionResult> GetTopFavoriteJokesForUser()
        {
            var userFavoriteJokes = await this.jokeService.GetTopFavoriteJokesForUserAsync(this.UserId);
            return Ok(userFavoriteJokes);
        }
    }
}
