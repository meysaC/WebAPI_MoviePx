using api.Data;
using api.Dtos.User;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;
        //private readonly IOMDbService _omdbService;
        private readonly ITmdbService _tmdbService;
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(ApplicationDBContext context, ITmdbService tmdbService,  UserManager<AppUser> userManager)
        {
            _context = context;
            //_omdbService = omdbService;
            _tmdbService =tmdbService;
            _userManager = userManager;
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
            var movieIds = favorites.Select(a => a.MovieId).ToList();
            
            for (int i = 0; i < favoritesDto.Count; i++)
            {
                //var movie = await _omdbService.GetMovieByIdAsync(movieMovieIds[i]);       
                var movie = await _tmdbService.GetMovieByIdAsync(movieIds[i]);       
                if (movie != null)
                {
                    favoritesDto[i].Title = movie.Title;
                    // favoritesDto[i].Director = movie.Director;
                    // favoritesDto[i].imdbRating = movie.imdbRating;
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

            //var movie = await _omdbService.GetMovieByIdAsync(favorite.ImdbID);          
            var movie = await _tmdbService.GetMovieByIdAsync(favorite.MovieId);          
            {
                favoriteDto.Title = movie.Title;
                // favoriteDto.Director = movie.Director;
                // favoriteDto.imdbRating = movie.imdbRating;
            }

            return favoriteDto;
        }
 
        public async Task<UserFollow> FallowUserAsync(UserFollow followModel)
        {
            await _context.UserFollows.AddAsync(followModel);
            await _context.SaveChangesAsync();
            return followModel;
        }

        public async Task<FollowDto> GetFollowByFollowIdAsync(int id)
        {
            var follow = await _context.UserFollows.Include(a => a.AppUser).FirstOrDefaultAsync(a => a.Id == id);
            if(follow == null) return null;

            var followDto = new FollowDto();
            followDto = follow.UserFollowToFollowDto();
            // {
            //     var years = (int)(timeSpan.TotalDays / 365);
            //     followDto.Since = $"{years} yıl önce.";
            // }

            var followingUser = await _userManager.FindByIdAsync(follow.FollowingId);
            followDto.FollowingUserName = followingUser.UserName;
            followDto.Since = SinceTime(follow.FollowedWhen);

            return followDto;
        }

        public async Task<UserFollow> UnFollowAsync(string followUserName, string appUserId)
        { 
            var followUser = await _userManager.FindByNameAsync(followUserName);
            var followModel = await _context.UserFollows
                                        .Where(a => a.AppUserId == appUserId && a.FollowingId == followUser.Id)
                                        .FirstOrDefaultAsync();;
            if(followModel == null ) return null;
            _context.Remove(followModel);
            await _context.SaveChangesAsync();
            return followModel;
        }

        public async Task<List<FollowDto?>> GetAllFollowsAsync(string id)
        {
            var follows = await _context.UserFollows
                                            .Include(a => a.AppUser)
                                            .Where(a => a.AppUserId == id)
                                            .ToListAsync();
            if(follows == null) return null;

            var followsDto = new List<FollowDto?>();
            followsDto = follows.Select(a => a.UserFollowToFollowDto()).ToList();
            
            var followingUserIds = follows.Select(a => a.FollowingId).ToList();
            var _followedWhens = follows.Select(a => a.FollowedWhen).ToList();
           
            for(int i = 0; i < followsDto.Count; i++)
            {
                var curFollow = await _context.Users.FirstOrDefaultAsync(a => a.Id == followingUserIds[i]);

                followsDto[i].FollowingUserName = curFollow.UserName; 
                followsDto[i].Since = SinceTime(_followedWhens[i]);
            }

            return followsDto;                                           
        }

        public string SinceTime(DateTime time)
        {
            var result = "";
            var timeSpan = DateTime.Now - time;
            if (timeSpan.TotalDays < 1)
            {
                result = "1 günden az süredir.";
            }
            else if (timeSpan.TotalDays < 30)
            {
                result = $"{(int)timeSpan.TotalDays} gün önce.";
            }
            else if (timeSpan.TotalDays < 365)
            {
                var months = (int)(timeSpan.TotalDays / 30);
                result = $"{months} ay önce.";
            }
            else
            {
                var years = (int)(timeSpan.TotalDays / 365);
                result = $"{years} yıl önce.";
            }
            return result;
        }
    }
}