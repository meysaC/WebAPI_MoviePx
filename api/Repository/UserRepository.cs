using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Account;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IOMDbService _omdbService;
        public UserRepository(ApplicationDBContext context, IOMDbService omdbService)
        {
            _context = context;
            _omdbService = omdbService;
        }
 
        public async Task<UserPreferance> AddFavoriteAsync(UserPreferance favoriteModel)
        {
            await _context.UserPreferances.AddAsync(favoriteModel);
            await _context.SaveChangesAsync();
            return favoriteModel;
        }

        public async Task<UserPreferance> DeleteFavoriteAsync(int id)
        {
            var accountModel = await _context.UserPreferances.FirstOrDefaultAsync(x => x.Id == id);
            if(accountModel == null ) return null;
            _context.Remove(accountModel);
            await _context.SaveChangesAsync();
            return accountModel;
        }

        public async Task<List<FavoriteDto?>> GetAllFavoritesAsync(string id) 
        {
            var favorites = await _context.UserPreferances
                                            .Include(a => a.AppUser)
                                            .Where(a => a.AppUserId == id)
                                            .ToListAsync();
            if(favorites == null) return null;                                                        
            var favoritesDto = favorites.Select(a => a.UserPreferanceToFavoriteDto()).ToList();
            var movieImbdIds = favorites.Select(a => a.ImdbID).ToList();
            
            for (int i = 0; i < favoritesDto.Count; i++)
            {
                var movie = await _omdbService.GetMovieByIdAsync(movieImbdIds[i]);       
                if (movie != null)
                {
                    favoritesDto[i].Title = movie.Title;
                    favoritesDto[i].Director = movie.Director;
                    favoritesDto[i].imdbRating = movie.imdbRating;
                }
            }            
            return favoritesDto;
        }

        public async Task<FavoriteDto> GetFavoriteByFavoriteIdAsync(int id)
        {
            var favorite = await _context.UserPreferances.FirstOrDefaultAsync(a => a.Id == id);
            if(favorite == null) return null;
            
            var favoriteDto = new FavoriteDto();
            favoriteDto = favorite.UserPreferanceToFavoriteDto();

            var movie = await _omdbService.GetMovieByIdAsync(favorite.ImdbID);
            
            {
                favoriteDto.Title = movie.Title;
                favoriteDto.Director = movie.Director;
                favoriteDto.imdbRating = movie.imdbRating;
            }
            return favoriteDto;

        }

    }
}