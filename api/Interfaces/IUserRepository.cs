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
        Task<AppUser> GetUserAsync(string id);

        //Task AddFavoriteAsync(UserFavorite favoriteModel); //<UserPreferance> geriye bir veri döndürmesine gerek yok o yüzden task 
        Task<Result<FavoriteDto>> AddFavoriteAsync(UserFavorite favorite); 
        Task<FavoriteDto?> GetFavoriteByFavoriteIdAsync(int favoriteId);
        Task<Result<string>> DeleteFavoriteAsync(int id); //<UserFavorite>
        Task<Result<List<FavoriteDto?>>> GetAllFavoritesAsync(string id); 
        
        Task<Result<WatchedDto>> AddWatchedAsync(UserWatched watched); //<UserPreferance>
        Task<WatchedDto?> GetWatchedByIdAsync(int watchedId);
        Task<Result<string>> DeleteWatchedAsync(int id); //<UserWatched>
        Task<Result<List<WatchedDto?>>> GetAllWatchedAsync(string id);

        Task<FollowDto> GetFollowByFollowIdAsync(int id);
        Task<UserFollow> FallowUserAsync(UserFollow followModel);
        Task<UserFollow> UnFollowAsync(string followUserName, string appUserId); 
        Task<List<FollowDto?>> GetAllFollowsAsync(string id); 

        string SinceTime (DateTime time );
    }
}