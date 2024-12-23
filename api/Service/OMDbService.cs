using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Movie;
using api.Helpers;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;


namespace api.Service
{
    public class OMDbService : IOMDbService
    {
        private HttpClient _httpClient;
        private IConfiguration _config;
        private readonly IRedisCacheService _redisCacheService;
        public OMDbService(HttpClient httpClient, IConfiguration config, IRedisCacheService redisCacheService)//
        {
            _httpClient = httpClient;
            _config = config;
            _redisCacheService = redisCacheService;
        }

        public async Task<MovieDetail?> GetMovieByIdAsync(string imdbId)
        {
            var cacheKey = $"MovieDetail:{imdbId}";
            var cachedMovie = await _redisCacheService.GetCacheAsync<MovieDetail>(cacheKey);
            if(cachedMovie != null) return cachedMovie;
            
            try
            {
                var result = await _httpClient.GetAsync($"{_config["OMDbSettings:BaseUrl"]}?i={imdbId}&apikey={_config["OMDbSettings:ApiKey"]}"); //a76fd39b
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var tasks = JsonConvert.DeserializeObject<MovieDetail>(content);
                    var movie = tasks;
                    if(movie != null && movie.Response == "True") 
                    {
                        _redisCacheService.SetCacheAsync<MovieDetail>(cacheKey, movie);
                        return movie;  ///mapper ile yapmam gerekiyor mu??????gerek yok zaten vb ye kaydetmiyoruz
                    }
                    return null;
                }
                return null;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                return null;
            }

        }

        public async Task<List<Search?>> SearchMoviesByTitleAsync(string title, int page = 1) 
        {
            var cacheKey = $"SearchMovies:{title}:{page}";
            var cachedMovies = await _redisCacheService.GetCacheAsync<List<Search>>(cacheKey);
            if(cachedMovies != null) return cachedMovies;

            try
            {
                var result = await _httpClient.GetAsync($"{_config["OMDbSettings:BaseUrl"]}?s={title}&page={page}&apikey={_config["OMDbSettings:ApiKey"]}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var searchResult = JsonConvert.DeserializeObject<SearchResult>(content);//OMdb API arama sonuçları düz bir dizi döndürmez; bunun yerine bir obje içinde bir liste döndürür
                    if (searchResult != null && searchResult.Response == "True")
                    {
                        // BAŞKA BİR METOD ÇAĞIRMAK PERFORMANSI DÜŞÜRÜR!!!!! BAŞKA ÇÖZÜM?????
                        var movies = searchResult.ToMovieFromOMDb(); //return;

                        foreach(var movie in movies)
                        {                           
                            var movieDetail = await GetMovieByIdAsync(movie.imdbID);
                            if(movieDetail != null) movie.Genre = movieDetail.Genre;
                        }
//BUNU YAPMAYA GEREK YOK ÇÜNKÜ GetMovieByIdAsync İLE MovieDetail TÜRÜNDE MOVİELERİ CACHELEDİK ZATEN VE var movieDetail = await GetMovieByIdAsync(movie.imdbID); İLE CACHELİ MOVİELERİ ÇEKİYORUZ BURDA LİSTEYİ CACHELEMEK GEREKSİZ OLUR???
                        //_redisCacheService.SetCacheAsync<List<Search>>(cacheKey, movies);  
                        return movies;
                    } 
                    return null;
                }
                return null;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Hata oluştu: {e.Message}");
                return null;
            }
        }

    }
}