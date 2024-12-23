using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IAccountrepository
    {
        Task<UserPreferance> AddFavoriteAsync(UserPreferance favoriteModel);
        Task<UserPreferance> DeleteFavoriteAsync(int id); 
        Task<FavoriteDto> GetFavoriteByFavoriteIdAsync(int id);
        Task<List<FavoriteDto?>> GetAllFavoritesAsync(string id); 
    }
}