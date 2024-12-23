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
    }
}