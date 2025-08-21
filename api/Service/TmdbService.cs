using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Movie;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Newtonsoft.Json;

namespace api.Service
{
    public class TmdbService : ITmdbService
    {
        private HttpClient _httpClient;
        private IConfiguration _config;
        private readonly IRedisCacheService _redisCacheService;

        public TmdbService(HttpClient httpClient, IConfiguration config, IRedisCacheService redisCacheService)
        {
            _httpClient = httpClient;
            _config = config;
            _redisCacheService = redisCacheService;
        }

        public async Task<Movie?> GetMovieByIdAsync(int MovieId)
        {
            var cacheKey = $"Movie:{MovieId}";
            var cachedMovie = await _redisCacheService.GetCacheAsync<Movie>(cacheKey);
            if (cachedMovie != null) return cachedMovie;

            try
            {
                var result = await _httpClient.GetAsync(
                                    $"{_config["TmdbSettings:BaseUrl"]}movie/{MovieId}?api_key={_config["TmdbSettings:ApiKey"]}&append_to_response=credits,videos,images,recommendations"); //?language=en-US

                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var tasks = JsonConvert.DeserializeObject<Movie>(content);
                    var movie = tasks;
                    if (movie != null)
                    {
                        _redisCacheService.SetCacheAsync<Movie>(cacheKey, movie);
                        return movie;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                return null;
            }
        }

        public async Task<List<Movie?>> SearchMoviesByTitleAsync(string title, int page = 1)
        {
            var cacheKey = $"SearchMovies:{title}:{page}";
            var cachedMovies = await _redisCacheService.GetCacheAsync<List<Movie>>(cacheKey);
            if (cachedMovies != null) return cachedMovies;

            try
            {
                var result = await _httpClient.GetAsync(
                                    $"{_config["TmdbSettings:BaseUrl"]}search/movie?api_key={_config["TmdbSettings:ApiKey"]}&query={title}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                    var movieResponse = JsonConvert.DeserializeObject<MovieResponse>(content);//OMdb API arama sonuçları düz bir dizi döndürmez; bunun yerine bir obje içinde bir liste döndürür
                    var movies = movieResponse.ToMovieFromTmdb();

                    foreach (var movie in movies)
                    {
                        var movieDetail = await GetMovieByIdAsync(movie.Id);
                        if (movieDetail != null) movie.Genres = movieDetail.Genres;
                    }
                    return movies;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                return null;
            }
        }

        //EmbeddingService bu method’u kullanarak filmlerin açıklamasını embedding’e çevirebilir.
        public async Task<List<MovieOverviewDto?>> GetPopularMoviesForEmbeddingAsync(int page = 1)
        {
            var result = await _httpClient.GetAsync(
                                $"{_config["TmdbSettings:BaseUrl"]}movie/popular?api_key={_config["TmdbSettings:ApiKey"]}&language=en-US&page={page}");
            if (!result.IsSuccessStatusCode) return new List<MovieOverviewDto?>();

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<MovieResponse>(content);

            //ilgili property leri mapliyoruz
            var movies = response.Results.Select(x => new MovieOverviewDto
            {
                Id = x.Id,
                Title = x.Title,
                Overview = x.Overview,
                GenreIds = x.Genres?.Select(g => g.Id).ToList()
            }).ToList();

            return movies;
        }
    }
}