using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface ITmdbService
    {
        Task<Movie?> GetMovieByIdAsync(int MovieId);
        Task<List<Movie?>> SearchMoviesByTitleAsync(string searchTerm, int page = 1); //string searchTerm
    }
}