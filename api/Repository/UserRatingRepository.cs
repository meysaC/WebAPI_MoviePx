using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.UserRating;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class UserRatingRepository : IUserRatingRepository
    {
        private readonly ApplicationDBContext _context;
        public UserRatingRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<UserRating>?> GetAllRatingsByImdbIDAsync(string imdbID)
        {
            var ratings = await _context.UserRatings
                                            .Include(a => a.AppUser)
                                            .Where(a => a.ImdbID == imdbID)
                                            .ToListAsync();
                                            return ratings;                                                      
        }
  
        public async Task<double> GetRatingRatioByImdbIDAsync(string imdbID)
        {
            var ratings =  await _context.UserRatings
                                            .Where(a => a.ImdbID == imdbID)
                                            .ToListAsync();

            var just_ratings = ratings.Select(a => a.Rate).ToList();
            
            double total = 0;
            foreach(double rate in just_ratings)
            {
                total += rate;
            }

            return total / just_ratings.Count();
        }

        public async Task<List<UserRating>?> GetAllRatingsForUserAsync(string userId)
        {
            return await _context.UserRatings
                                            .Include(a => a.AppUser)
                                            .Where(a => a.AppUserId == userId)
                                            .ToListAsync();                                                      
        }

        public async Task<UserRating?> GetRatingByIdAsync(int id)
        {
            return await _context.UserRatings.Include(a => a.AppUser).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<UserRating?> CreateAsync(UserRating userRatingModel)
        {
            await _context.UserRatings.AddAsync(userRatingModel);
            await _context.SaveChangesAsync();
            return userRatingModel;
        }

        public async Task<UserRating?> UpdateAsync(int id, double rate)
        {
            var existingRating = await _context.UserRatings.Include(c => c.AppUser).FirstOrDefaultAsync(a => a.Id == id);
            if(existingRating == null) return null;

            existingRating.Rate = rate;
            existingRating.UpdatedOn = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingRating;
        }

        public async Task<UserRating?> DeleteAsync(int id)
        {
            var existingRating = await _context.UserRatings.Include(c => c.AppUser).FirstOrDefaultAsync(a => a.Id == id);
            if(existingRating == null) return null;
            _context.Remove(existingRating);
            await _context.SaveChangesAsync();
            return existingRating;
        }
    }
}