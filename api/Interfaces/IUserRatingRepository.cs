using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.UserRating;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IUserRatingRepository
    {
        Task<UserRating?> GetRatingByIdAsync(int id);
        Task<List<UserRating?>> GetAllRatingsByMovieIdAsync(int MovieId);
        Task<double> GetRatingRatioByMovieIdAsync(int MovieId);
        //Task<List<UserRating?>> GetAllRatingsByImdbIDAsync(string imdbID);
        //Task<double> GetRatingRatioByImdbIDAsync(string imdbID);
        Task<List<UserRating?>> GetAllRatingsForUserAsync(string userId);
        Task<UserRating?> CreateAsync(UserRating userRatingModel);
        Task<UserRating?> UpdateAsync(int id, double rate);
        Task<UserRating?> DeleteAsync(int id);
    }
}