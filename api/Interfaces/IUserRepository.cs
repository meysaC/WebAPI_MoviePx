using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Models;

namespace api.Interfaces
{
    public interface IUserRepository
    {
        Task<UserPreferance> AddFavoriteAsync(UserPreferance favoriteModel);
        Task<UserPreferance> DeleteFavoriteAsync(int id); 
        Task<FavoriteDto> GetFavoriteByFavoriteIdAsync(int id);
        Task<List<FavoriteDto?>> GetAllFavoritesAsync(string id); 
        
        Task<FollowDto> GetFollowByFollowIdAsync(int id);
        Task<UserFollow> FallowUserAsync(UserFollow followModel);
        Task<UserFollow> UnFollowAsync(string followUserName, string appUserId); 
        Task<List<FollowDto?>> GetAllFollowsAsync(string id); 

        string SinceTime (DateTime time );
    }
}