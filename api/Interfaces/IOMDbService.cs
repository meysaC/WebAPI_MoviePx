using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Movie;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IOMDbService
    {
        Task<MovieDetail?> GetMovieByIdAsync(string imdbId);
        Task<List<Search?>> SearchMoviesByTitleAsync(string searchTerm, int page = 1); //string searchTerm
    }
}