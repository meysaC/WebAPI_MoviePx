using api.Data;
using api.Dtos.Movie;
using api.Dtos.User;
using api.Interfaces;
using api.Mapper;
using api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDBContext context, ITmdbService tmdbService, UserManager<AppUser> userManager, IMapper mapper)
        {
            _context = context;
            //_omdbService = omdbService;
            _tmdbService = tmdbService;
            _userManager = userManager;
            _mapper = mapper;
        }
 
        public async Task<AppUser> GetUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null) return null;
            return user;
        }


        public async Task<Result<FavoriteDto>> AddFavoriteAsync(UserFavorite favorite) //<UserPreferance>UserPreferance
        {
            // var existingFavorite = await _context.UserFavorites
            //     .FirstOrDefaultAsync(f => f.AppUserId == favoriteModel.AppUserId && f.MovieId == favoriteModel.MovieId); 
            // if (existingFavorite == null)
            // {
            //     //return existingFavorite;
            //     await _context.UserFavorites.AddAsync(favoriteModel);
            //     await _context.SaveChangesAsync();
            // }
            //return favoriteModel;


            var exists = await _context.UserFavorites
                .AnyAsync(f => f.AppUserId == favorite.AppUserId && f.MovieId == favorite.MovieId);
            if (exists) return Result<FavoriteDto>.FailResult("This movie is already in your favorites.");

            favorite.CreatedAt = DateTime.UtcNow; // Set CreatedAt to current time
            await _context.UserFavorites.AddAsync(favorite);
            await _context.SaveChangesAsync();

            var favoriteDto = _mapper.Map<FavoriteDto>(favorite);
            return Result<FavoriteDto>.SuccessResult(favoriteDto);
        }
        public async Task<FavoriteDto?> GetFavoriteByFavoriteIdAsync(int favoriteId)
        {
            // var favorite = await _context.UserPreferances.FirstOrDefaultAsync(a => a.Id == id);
            // if(favorite == null) return null;

            // var favoriteDto = new FavoriteDto();
            // favoriteDto = favorite.UserPreferanceToFavoriteDto();

            // //var movie = await _omdbService.GetMovieByIdAsync(favorite.ImdbID);          
            // var movie = await _tmdbService.GetMovieByIdAsync(favorite.MovieId);          
            // {
            //     //favoriteDto.Title = movie.Title;
            //     // favoriteDto.Director = movie.Director;
            //     // favoriteDto.imdbRating = movie.imdbRating;
            // }
            // return favoriteDto;

            var favorite = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.Id == favoriteId);
            return favorite == null ? null : _mapper.Map<FavoriteDto>(favorite);
        }
        public async Task<Result<string>> DeleteFavoriteAsync(int favoriteId)
        {
            var favorite = await _context.UserFavorites.FirstOrDefaultAsync(x => x.Id == favoriteId);
            if (favorite == null) return Result<string>.SuccessResult("Favori bulunamadı.");
            _context.UserFavorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return Result<string>.SuccessResult("Favori başarıyla silindi.");
        }
        public async Task<Result<List<FavoriteDto?>>> GetAllFavoritesAsync(string id)
        {
            // var favorites = await _context.UserPreferances
            //                                 .Where(a => a.AppUserId == id)
            //                                 .ToListAsync();
            // if(favorites == null) return null;                                                        
            // var favoritesDto = favorites.Select(a => a.UserPreferanceToFavoriteDto()).ToList();
            // var movieIds = favorites.Select(a => a.MovieId).ToList();

            // for (int i = 0; i < favoritesDto.Count; i++)
            // {
            //     var movie = await _tmdbService.GetMovieByIdAsync(movieIds[i]);
            //     if (movie != null)
            //     {
            //         //favoritesDto[i].Movie = movie;
            //     }
            // }            
            // return favoritesDto;

            var favorites = await _context.UserFavorites
                .Where(f => f.AppUserId == id)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            if (favorites == null || !favorites.Any()) return Result<List<FavoriteDto?>>.FailResult("No favorites found.");
            var favoriteDtos = _mapper.Map<List<FavoriteDto>>(favorites);

            var movieTasks = favorites.Select(f => _tmdbService.GetMovieByIdAsync(f.MovieId)).ToList();
            //TMDb verilerini aynı anda (paralel olarak) çekmek, Bütün istekler aynı anda başlar, CPU veya thread beklemez, .NET async IO ile tümünü aynı anda bekler
            var movieResults = await Task.WhenAll(movieTasks);

            for (int i = 0; i < favorites.Count; i++)
            {
                if (movieResults[i] != null)
                {
                    favoriteDtos[i].Movie = _mapper.Map<MovieDto>(movieResults[i]);
                }
            }
            return Result<List<FavoriteDto?>>.SuccessResult(favoriteDtos);
        }


        public async Task<Result<WatchedDto>> AddWatchedAsync(UserWatched watched)
        {
            var exists = await _context.UserWatcheds
                .AnyAsync(w => w.AppUserId == watched.AppUserId && w.MovieId == watched.MovieId);
            if (exists) return Result<WatchedDto>.FailResult("This movie is already in your watched.");
            watched.CreatedAt = DateTime.UtcNow;
            await _context.UserWatcheds.AddAsync(watched);
            await _context.SaveChangesAsync();
            var watchedDto = _mapper.Map<WatchedDto>(watched);
            return Result<WatchedDto>.SuccessResult(watchedDto); 
        }
        public async Task<WatchedDto?> GetWatchedByIdAsync(int watchedId)
        {
            var watched = await _context.UserWatcheds
                .FirstOrDefaultAsync(w => w.Id == watchedId);
            return watched == null ? null : _mapper.Map<WatchedDto>(watched);
        }
        public async Task<Result<string>> DeleteWatchedAsync(int id)
        {
            var watchedModel = await _context.UserWatcheds.FirstOrDefaultAsync(x => x.Id == id);
            if (watchedModel == null) return Result<string>.SuccessResult("İzlenen film başarıyla silindi.");
            _context.Remove(watchedModel);
            await _context.SaveChangesAsync();
            return Result<string>.SuccessResult("İzlenen film başarıyla silindi.");
        }
        public async Task<Result<List<WatchedDto?>>> GetAllWatchedAsync(string id)
        {
            var watcheds = await _context.UserWatcheds
                .Where(w => w.AppUserId == id)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
            if (watcheds == null || !watcheds.Any()) return Result<List<WatchedDto?>>.FailResult("No watched movies found.");
            var watchedDtos = _mapper.Map<List<WatchedDto?>>(watcheds);
            var movieTasks = watcheds.Select(w => _tmdbService.GetMovieByIdAsync(w.MovieId)).ToList();
            var movieResults = await Task.WhenAll(movieTasks);
            for (int i = 0; i < watcheds.Count; i++)
            {
                if (movieResults[i] != null)
                {
                    watchedDtos[i].Movie = _mapper.Map<MovieDto>(movieResults[i]);
                }
            }
            return Result<List<WatchedDto?>>.SuccessResult(watchedDtos);
        }


        public async Task<UserFollow> FallowUserAsync(string followerId, string followingId) // FOLLOWER = ŞU AN UYGULAMAYI KULLANAN KULLANICI ,  FOLOWİNG = TAKİP ETTİĞİ KULLANICI !!
        {
            //front a da kendini takip etmeyi kontrol edicez ama burda da kontrol etmek gerekiyor (network tab den istek atılabilir front devre dışı bırakilıp)
            if (followerId == followingId) throw new InvalidOperationException("You can not follow yourself.");
            var follow = await _context.UserFollows
                                .FirstOrDefaultAsync(f => f.FollowingId == followingId && f.FollowerId == followerId);

            if (follow == null)
            {
                follow = new UserFollow //ilk defa kontrol ediliyosa
                {
                    FollowerId = followerId,
                    FollowingId = followingId,
                    IsActive = true,
                    FollowedWhen = DateTime.UtcNow
                };
                _context.UserFollows.Add(follow);
            }
            else
            {
                follow.IsActive = !follow.IsActive;
                follow.UnFollowedWhen = follow.IsActive ? null : DateTime.UtcNow;
                _context.UserFollows.Update(follow);
            }
            await _context.SaveChangesAsync();
            return follow;
        }
        public async Task<IEnumerable<FollowDto>> GetUserFollowingsAsync(string userId) // follower -->şu anki kullanıcı 
        {
            return await _context.UserFollows
                    .Include(x => x.Following) //follower ve following null gelir
                    .Where(x => x.FollowerId == userId && x.IsActive)
                    .ProjectTo<FollowDto>(_mapper.ConfigurationProvider) //ProjectTo AutoMapper ile birlikte EF Core’da kullanıldığında performanslı olur,Tüm entity’yi memory’e çekmez,EF Core sadece ihtiyaç duyulan alanları çeker
                    .ToListAsync();
        }
        public async Task<IEnumerable<FollowDto>> GetUserFollowersAsync(string userId)
        {
            return await _context.UserFollows
                    .Include(x => x.Follower)
                    .Where(x => x.FollowingId == userId && x.IsActive)
                    .ProjectTo<FollowDto>(_mapper.ConfigurationProvider) 
                    .ToListAsync();
        }

        // public async Task<FollowDto> GetFollowByFollowIdAsync(int id) //bu id kullanıcı id değil 
        // {
        //     var follow = await _context.UserFollows.Include(a => a.AppUser).FirstOrDefaultAsync(a => a.Id == id);
        //     if(follow == null) return null;

        //     var followDto = new FollowDto();
        //     followDto = follow.UserFollowToFollowDto();
        //     // {
        //     //     var years = (int)(timeSpan.TotalDays / 365);
        //     //     followDto.Since = $"{years} yıl önce.";
        //     // }

        //     var followingUser = await _userManager.FindByIdAsync(follow.FollowingId);
        //     followDto.FollowingUserName = followingUser.UserName;
        //     followDto.Since = SinceTime(follow.FollowedWhen);

        //     return followDto;
        // }
        // public async Task<UserFollow> UnFollowAsync(string followUserName, string appUserId)
        // { 
        //     var followUser = await _userManager.FindByNameAsync(followUserName);
        //     var followModel = await _context.UserFollows
        //                                 .Where(a => a.AppUserId == appUserId && a.FollowingId == followUser.Id)
        //                                 .FirstOrDefaultAsync();;
        //     if(followModel == null ) return null;
        //     _context.Remove(followModel);
        //     await _context.SaveChangesAsync();
        //     return followModel;
        // }
        // public async Task<List<FollowDto?>> GetAllFollowsAsync(string id)
        // {
        //     var follows = await _context.UserFollows
        //                                     .Include(a => a.AppUser)
        //                                     .Where(a => a.AppUserId == id)
        //                                     .ToListAsync();
        //     if(follows == null) return null;

        //     var followsDto = new List<FollowDto?>();
        //     followsDto = follows.Select(a => a.UserFollowToFollowDto()).ToList();
            
        //     var followingUserIds = follows.Select(a => a.FollowingId).ToList();
        //     var _followedWhens = follows.Select(a => a.FollowedWhen).ToList();
           
        //     for(int i = 0; i < followsDto.Count; i++)
        //     {
        //         var curFollow = await _context.Users.FirstOrDefaultAsync(a => a.Id == followingUserIds[i]);

        //         followsDto[i].FollowingUserName = curFollow.UserName; 
        //         followsDto[i].Since = SinceTime(_followedWhens[i]);
        //     }

        //     return followsDto;                                           
        // }


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