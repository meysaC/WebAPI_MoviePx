using System.IdentityModel.Tokens.Jwt;
using api.Dtos.User;
using api.Extensions;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/user")]
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

        [HttpGet("get_user")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userRepo.GetUserAsync(id);
           if (user == null) return NotFound("The user doesn't exist.");

            return Ok(user);
        }



        //[AllowAnonymous] //kimlik doğrulama gerekmiyor
        [HttpGet("all_favorites/{userId}")]
        public async Task<IActionResult> GetAllFavorite([FromRoute] string userId)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);
            
            // user id yi aldğımız için gerek yok buna
            // var userName = User.GetUsername();
            // var appUser = await _userManager.FindByNameAsync(userName);

            var favorites = await _userRepo.GetAllFavoritesAsync(userId);
            if(favorites == null) return NoContent();
            return Ok(favorites); //MovieDto TİPİNDE DÖNDÜRÜYOR FRONTEND E !!!!!
        }
        
        [HttpGet("favorite/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteByFavoriteId([FromRoute] int id )
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var favorite = await _userRepo.GetFavoriteByFavoriteIdAsync(id);  
            if(favorite == null) return NotFound("Doesn't exists.");

            return Ok(favorite);  //favorite.ToFavoriteDto()
        }

        [HttpPost("add_favorite/{MovieId}")] //ImdbID
        //[Route("{ImdbID}")]
        [Authorize]
        public async Task<IActionResult> CreateFavorite([FromRoute] int MovieId) //[FromQuery] string title, [FromBody] FavoriteQueryObject queryObject
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            // var favoriteDto = new FavoriteDto();            

            // var favoriteModel = favoriteDto.ToUserPreferanceFromFavoriteDto();
            // favoriteModel.MovieId = MovieId;
            // favoriteModel.AppUserId = appUser.Id;

            // await _userRepo.AddFavoriteAsync(favoriteModel);

            // favoriteDto =  await _userRepo.GetFavoriteByFavoriteIdAsync(favoriteModel.Id);

            // //return Ok(); //result.UserPreferanceToFavoriteDto() //Bu, UI'da kullanılabilirliği artırır. Ayrıca 201 Created yerine genelde 200 OK ile basit bir cevap UI için yeterlidir (özellikle sayfaya redirect etmiyorsan).
            // return CreatedAtAction(nameof(GetFavoriteByFavoriteId), new { id = favoriteModel.Id}, favoriteDto); //,   favoriteModel.ToFavoriteDto()            



            var favoriteModel = new UserFavorite
            {
                MovieId = MovieId,
                AppUserId = appUser.Id
            };
            var result = await _userRepo.AddFavoriteAsync(favoriteModel);
            if (!result.Success) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetFavoriteByFavoriteId), new { id = result.Data.Id }, result.Data);
        }

        [HttpDelete("remove_favorite/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFavorite([FromRoute] int id)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var favoriteModel = await _userRepo.DeleteFavoriteAsync(id);
            if(favoriteModel == null) return NotFound("Not succsessfull request.");

            return NoContent();
        }



        [HttpPost("add_watched/{MovieId}")]
        [Authorize]
        public async Task<IActionResult> AddWatched([FromRoute] int MovieId)
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);
            var watchedModel = new UserWatched
            {
                MovieId = MovieId,
                AppUserId = appUser.Id
            };
            var result = await _userRepo.AddWatchedAsync(watchedModel);
            if (!result.Success) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetWatchedByIdAsync), new { id = result.Data.Id }, result.Data);

        }
        [HttpGet("watched/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetWatchedByIdAsync([FromRoute] int id )
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var watched = await _userRepo.GetWatchedByIdAsync(id);  
            if(watched == null) return NotFound("Doesn't exists.");

            return Ok(watched);
        }

        [HttpGet("all_watched/{userId}")]
        public async Task<IActionResult> GetAllWatched([FromRoute] string userId)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);
            var watcheds = await _userRepo.GetAllWatchedAsync(userId);
            if(watcheds == null) return NoContent();
            return Ok(watcheds); 
        }
        [HttpDelete("remove_watched/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteWatched([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var watchedModel = await _userRepo.DeleteWatchedAsync(id);
            if (watchedModel == null) return NotFound("Not succsessfull request.");
            return NoContent();
        }



        [HttpGet("follow/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetFollowByFollowId([FromRoute] int id )
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var favorite = await _userRepo.GetFollowByFollowIdAsync(id);  
            if(favorite == null) return NotFound("Doesn't exists.");

            return Ok(favorite);
        }

        [HttpGet("followings")]
        public async Task<IActionResult> GetAllFollowing() 
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var follows = await _userRepo.GetAllFollowsAsync(appUser.Id);  
            if(follows == null) return NoContent();
            return Ok(follows); 
        }

        [HttpPost("follow/{username}")]
        [Authorize]
        public async Task<IActionResult> Follow([FromRoute] string username)
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);
            var followUser = await _userManager.FindByNameAsync(username);


            var followDto = new FollowDto();
            var followModel = followDto.ToUserFollowFromFollowDto();
            followModel.AppUserId = appUser.Id;
            followModel.FollowingId = followUser.Id;

            await _userRepo.FallowUserAsync(followModel);

            followDto = await _userRepo.GetFollowByFollowIdAsync(followModel.Id);

            return CreatedAtAction(nameof(GetFollowByFollowId), new { id = followModel.Id}, followDto); //,   favoriteModel.ToFavoriteDto()            
        }

        [HttpDelete("unfollow/{username}")]
        [Authorize]
        public async Task<IActionResult> UnFollow([FromRoute] string username)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var followModel = await _userRepo.UnFollowAsync(username, appUser.Id);
            if(followModel == null) return NotFound("Not succsessfull request.");

            return NoContent();
        }

    }
}