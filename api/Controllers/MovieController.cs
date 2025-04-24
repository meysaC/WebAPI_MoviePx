using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IOMDbService _oMDbService;
        private readonly ITmdbService _tmdbService;

        public MovieController(ITmdbService tmdbService) //IOMDbService oMDbService
        {
            //_oMDbService = oMDbService;
            _tmdbService = tmdbService;
        }

        [HttpGet("{MovieId}")] 
        public async Task<IActionResult> GetMovieDetailsById(int MovieId)
        {
            //var movie = await _oMDbService.GetMovieByIdAsync(MovieId);
            var movie = await _tmdbService.GetMovieByIdAsync(MovieId);
            if (movie == null) return NotFound();
            return Ok(movie);
        }

        [HttpGet("search_by_title")]
        public async Task<IActionResult> SearchMoviesByTitle([FromQuery] string title, [FromQuery] int page = 1) //
        {
            var results = await _tmdbService.SearchMoviesByTitleAsync(title, page); //title
            if (results == null) return NotFound();
            return Ok(results);
        }
                
    }
}