using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Domain.DTOs.CategoryDTOS;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Services.Abstraction;
using System.Linq;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Controllers
{
    [ApiController]
    [Route("api/jokes")]
    public class JokeControlles : BaseApiController
    {
        private readonly IJokeService jokeService;
        private readonly ICategoryService categoryService;

        public JokeControlles(IJokeService jokeService, ICategoryService categoryService)
        {
            this.jokeService = jokeService;
            this.categoryService = categoryService;
        }

        [HttpGet]
        [Route("random")]
        public async Task<IActionResult> GetRandomJoke([FromQuery] string category)
        {
            var jokeDto = await jokeService.GetRandomJokeAsync(category);
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
            var categories = await categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("categories/exists")]
        public async Task<IActionResult> CategoryExists([FromQuery] string categoryTitle)
        {
            var categories = await categoryService.CategoryExistsAsync(categoryTitle);
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
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
            await jokeService.AddJokeToFavoriteAsync(favoriteJokeId, this.UserId);
            return CreatedAtAction(nameof(AddJokeToFavorite), "We added your joke to favorite");
        }

        [HttpDelete]
        [Route("favorite/{favoriteJokeId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteJokefromFavorite([FromRoute] int favoriteJokeId)
        {
            await jokeService.DeleteJokeFromFavoriteAsync(favoriteJokeId, this.UserId);
            return Ok();
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

        [HttpGet]
        public async Task<IActionResult> GetAllJokes()
        {
            var jokes = await this.jokeService.GetAllJokesAsync();
            return Ok(jokes);
        }

        [HttpGet]
        [Route("{jokeId}")]
        public async Task<IActionResult> GetJoke([FromRoute]int jokeId)
        {
            var joke = await this.jokeService.GetJokeAsync(jokeId);
            return Ok(joke);
        }

        [HttpPut]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateJoke(UpdateJokeDto updateJokeDto)
        {
            var jokeDto = await this.jokeService.UpdateJokeAsync(updateJokeDto);
            return Ok(jokeDto);
        }

        [HttpDelete]
        [Route("{jokeId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteJoke([FromRoute]int jokeId)
        {
            await this.jokeService.DeleteJokeAsync(jokeId);
            return Ok();
        }

        [HttpPost]
        [Route("categories")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDTO)
        {
            var categoryDto = await categoryService.CreateCategoryAsync(createCategoryDTO);
            return CreatedAtAction(nameof(CreateCategory), categoryDto);
        }

        [HttpDelete]
        [Route("categories/{categoryId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteCategory([FromRoute]int categoryId)
        {
            await this.categoryService.DeleteCategoryAsync(categoryId);
            return Ok();
        }

        [HttpGet]
        [Route("favorite/top")]
        public async Task<IActionResult> GetFavoriteJokes()
        {
            var jokes = await this.jokeService.GetTopFavoriteJokesAsync();
            return Ok(jokes);
        }
    }
}
