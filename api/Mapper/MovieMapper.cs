using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Movie;
using api.Models;

namespace api.Mapper
{
    public static class MovieMapper
    {
        //OMDB İÇİN!!!!!!!!!!!
        public static List<Search> ToMovieFromOMDb(this SearchResult searchResult) //SearchMovie
        {
            return searchResult.Search.Select(movie => new Search
            {
                Title = movie.Title,
                Year = movie.Year,
                Genre = movie.Genre,
                imdbID = movie.imdbID,
                Type = movie.Type,
                Poster = movie.Poster
            }).ToList();
        }
        public static List<Movie> ToMovieFromTmdb(this MovieResponse movieResponse) //SearchMovie
        {
            return movieResponse.Results.Select(movie => new Movie
            {
                Id = movie.Id,
                Title = movie.Title,
                OriginalTitle = movie.OriginalTitle,
                OriginalLanguage = movie.OriginalLanguage,
                Overview = movie.Overview,
                ReleaseDate = movie.ReleaseDate,
                Genres = movie.Genres,
                BackdropPath = movie.BackdropPath,
                PosterPath = movie.PosterPath,
                Popularity = movie.Popularity,
                VoteAverage = movie.VoteAverage,
                VoteCount = movie.VoteCount,
                Adult = movie.Adult,
                Video = movie.Video,
                // Credits = movie.Credits,
                // Videos = movie.Videos,
                // Images = movie.Images,
                // Reviews = movie.Reviews,
                // Recommendations = movie.Recommendations
            }).ToList();
        }
    }
}