using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Movie;
using api.Helpers;
using api.Interfaces;
using api.Service;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;

namespace api.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        //private readonly IOMDbService _oMDbService;
        private readonly ITmdbService _tmdbService;
        private readonly api.Interfaces.IEmbeddingService _embeddingService;
        private readonly VectorDbService _vectorDbService;
        private readonly IOpenAIService _openAiService;

        public MovieController(ITmdbService tmdbService, api.Interfaces.IEmbeddingService embeddingService, VectorDbService vectorDbService, IOpenAIService openAiService) //IOMDbService oMDbService
        {
            //_oMDbService = oMDbService;
            _tmdbService = tmdbService;
            _embeddingService = embeddingService;
            _vectorDbService = vectorDbService;
            _openAiService = openAiService;
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

        [HttpPost("recommendations")]
        public async Task<IActionResult> RecommendMovies([FromBody] MovieRecommendationDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Description)) return BadRequest("Description cannot be empty.");

            //kullanıcı tarifini embedding çevir
            var userEmbedding = await _embeddingService.GetEmbeddingAsync(requestDto.Description);

            //vector db den bezneri filmleri al
            var similarMovies = await _vectorDbService.GetSimilarMoviesAsync(userEmbedding, topN: 10); // 10 tane benzer film al

            // gpt ile doğal dilde öneri oluştur
            var prompt = $@"You are a movie recommendation assistant.
            User description: {requestDto.Description}
            Suggested movies (title + overview):
            {string.Join("\n", similarMovies.Select(m => $"{m.title}: {m.overview}"))}
            Provide a short natural language recommendation based on the above movies.";

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
            //Eğer ileride “mod = sad, ama aynı zamanda geçen ay izlediği filmleri de dahil et” gibi daha fazla kontekstle çalışmak istersen o zaman chat endpoint’e (gpt-3.5-turbo) geçersin.
            var completionRequest = new OpenAI.ObjectModels.RequestModels.CompletionCreateRequest
            {
                Model = "gpt-3.5-turbo-instruct",//OpenAI.ObjectModels.Models.TextDavinciV3,//"gpt-3.5-turbo",
                Prompt = prompt,
                MaxTokens = 150
                //Temperature = 0.7f //0.7 yaparak daha yaratıcı 0.2 yaparak daha deterministik sonuç
            };
            var gptResponse = await _openAiService.Completions.CreateCompletion(completionRequest);
            
            if (gptResponse == null || gptResponse.Choices == null || !gptResponse.Choices.Any()) return BadRequest("Failed to generate recommendation from GPT.");

            

            return Ok(new
            {
                RecommendMovies = similarMovies.Select(m => new { m.movie_id, m.title, m.overview, }), //m.poster_path 
                NaturalRecommendation = gptResponse.Choices.FirstOrDefault()?.Text//?.Trim() ?? "No recommendation available."
            });

        }      
    }
}