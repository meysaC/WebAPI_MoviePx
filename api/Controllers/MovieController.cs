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

        public MovieController(IOMDbService oMDbService)
        {
            _oMDbService = oMDbService;
        }

        [HttpGet("{imdbId}")]
        public async Task<IActionResult> GetMovieDetailsById(string imdbId)
        {
            var movie = await _oMDbService.GetMovieByIdAsync(imdbId);
            if (movie == null) return NotFound();
            return Ok(movie);
        }

        [HttpGet("search_by_title")]
        public async Task<IActionResult> SearchMoviesByTitle([FromQuery] string title, [FromQuery] int page = 1) //
        {
            var results = await _oMDbService.SearchMoviesByTitleAsync(title, page); //title
            if (results == null) return NotFound();
            return Ok(results);
        }        
    }
}