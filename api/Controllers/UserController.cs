using System.IdentityModel.Tokens.Jwt;
using api.Dtos.User;
using api.Dtos.User.Favorite;
using api.Dtos.User.Watched;
using api.Extensions;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/users")] //Kaynak adı çoğul olmalı
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepo;

        public UserController(UserManager<AppUser> userManager, IUserRepository userRepo)
        {
            _userManager = userManager;
            _userRepo = userRepo;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] string userId)
        {
            var user = await _userRepo.GetUserAsync(userId);
            if (user == null) return NotFound("The user doesn't exist.");
            return Ok(user);
        }



        //[AllowAnonymous] //kimlik doğrulama gerekmiyor
        [HttpGet("{userId}/favorites")] //all_favorites/
        public async Task<IActionResult> GetAllFavorite([FromRoute] string userId) //[FromQuery] string title, [FromBody] FavoriteQueryObject queryObject
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // user id yi aldğımız için gerek yok buna
            // var userName = User.GetUsername();
            // var appUser = await _userManager.FindByNameAsync(userName);

            var favorites = await _userRepo.GetAllFavoritesAsync(userId);
            if (favorites == null) return NoContent();
            return Ok(favorites); //MovieDto TİPİNDE DÖNDÜRÜYOR FRONTEND E !!!!!
        }

        [HttpPost("favorites")] //add_favorite/{MovieId}
        [Authorize]
        public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteDto favoriteDto) //[FromRoute] int MovieId
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            /* AutoMapper öncesi
            // var favoriteDto = new FavoriteDto();            
            // var favoriteModel = favoriteDto.ToUserPreferanceFromFavoriteDto();
            // favoriteModel.MovieId = MovieId;
            // favoriteModel.AppUserId = appUser.Id;
            // await _userRepo.AddFavoriteAsync(favoriteModel);
            // favoriteDto =  await _userRepo.GetFavoriteByFavoriteIdAsync(favoriteModel.Id);
            // //return Ok(); //result.UserPreferanceToFavoriteDto() //Bu, UI'da kullanılabilirliği artırır. Ayrıca 201 Created yerine genelde 200 OK ile basit bir cevap UI için yeterlidir (özellikle sayfaya redirect etmiyorsan).
            // return CreatedAtAction(nameof(GetFavoriteByFavoriteId), new { id = favoriteModel.Id}, favoriteDto); //,   favoriteModel.ToFavoriteDto()            
            */

            var favoriteModel = new UserFavorite
            {
                MovieId = favoriteDto.MovieId,
                AppUserId = appUser.Id
            };
            var result = await _userRepo.AddFavoriteAsync(favoriteModel);
            if (!result.Success) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetFavoriteByFavoriteId), new { id = result.Data.Id }, result.Data);
        }

        [HttpGet("favorites/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteByFavoriteId([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var favorite = await _userRepo.GetFavoriteByFavoriteIdAsync(id);
            if (favorite == null) return NotFound("Doesn't exists.");

            return Ok(favorite);  //favorite.ToFavoriteDto()
        }

        [HttpDelete("favorites/{id:int}")] //remove_favorite
        [Authorize]
        public async Task<IActionResult> DeleteFavorite([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var favoriteModel = await _userRepo.DeleteFavoriteAsync(id);
            if (favoriteModel == null) return NotFound("Not succsessfull request.");

            return NoContent();
        }



        [HttpGet("{userId}/watcheds")]
        public async Task<IActionResult> GetAllWatched([FromRoute] string userId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var watcheds = await _userRepo.GetAllWatchedAsync(userId);
            if (watcheds == null) return NoContent();
            return Ok(watcheds);
        }

        [HttpPost("watcheds")]
        [Authorize]
        public async Task<IActionResult> AddWatched([FromBody] CreateWatchedDto watchedDto)
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);
            var watchedModel = new UserWatched
            {
                MovieId = watchedDto.MovieId,
                AppUserId = appUser.Id
            };
            var result = await _userRepo.AddWatchedAsync(watchedModel);
            if (!result.Success) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetWatchedByIdAsync), new { id = result.Data.Id }, result.Data);

        }
        [HttpGet("watcheds/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetWatchedByIdAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var watched = await _userRepo.GetWatchedByIdAsync(id);
            if (watched == null) return NotFound("Doesn't exists.");

            return Ok(watched);
        }

        [HttpDelete("watcheds/{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteWatched([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var watchedModel = await _userRepo.DeleteWatchedAsync(id);
            if (watchedModel == null) return NotFound("Not succsessfull request.");
            return NoContent();
        }



        // [HttpGet("follow/{id:int}")]
        // [Authorize]
        // public async Task<IActionResult> GetFollowByFollowId([FromRoute] int id )
        // {
        //     if(!ModelState.IsValid ) return BadRequest(ModelState);

        //     var favorite = await _userRepo.GetFollowByFollowIdAsync(id);  
        //     if(favorite == null) return NotFound("Doesn't exists.");

        //     return Ok(favorite);
        //}

        [HttpPost("{followingUserId}/follows")]
        [Authorize]
        public async Task<IActionResult> Follow([FromRoute] string followingUserId) 
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null) return Unauthorized();

            var followingUser = await _userManager.FindByIdAsync(followingUserId);
            if (followingUser == null) return NotFound("The follower does not exist.");

            var follow = await _userRepo.FallowUserAsync(appUser.Id, followingUserId);

            return Ok(new
            {
                followId = follow.Id,
                isFollowing = follow.IsActive,
                FollowedWhen = follow.FollowedWhen,
                UnFollowedWhen = follow.UnFollowedWhen
            });
        }
        [HttpGet("{userId}/followings")]
        public async Task<IActionResult> GetAllFollowings([FromRoute] string userId)
        {
            var followings = await _userRepo.GetUserFollowingsAsync(userId);
            return Ok(followings);
        }   
        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetAllFollowers([FromRoute] string userId) 
        {
            var followers = await _userRepo.GetUserFollowersAsync(userId);
            return Ok(followers);
        }

        // [HttpDelete("unfollow/{username}")]
        // [Authorize]
        // public async Task<IActionResult> UnFollow([FromRoute] string username)
        // {
        //     if(!ModelState.IsValid ) return BadRequest(ModelState);

        //     var userName = User.GetUsername();
        //     var appUser = await _userManager.FindByNameAsync(userName);

        //     var followModel = await _userRepo.UnFollowAsync(username, appUser.Id);
        //     if(followModel == null) return NotFound("Not succsessfull request.");

        //     return NoContent();
        // }

    }
}