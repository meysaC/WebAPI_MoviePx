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
        
        [HttpGet("favorites")]
        public async Task<IActionResult> GetAllFavorite()  //ASLINDA BU TÜM KULLANICILARIN DİĞER KULLANICILARIN FAVORİLERİNE BAKMASI OLMALI O YÜZDEN APPUSER.ID DEĞİL DE KİME TIKLANDIYA ONUN İD Sİ GİTMELİ
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var favorites = await _userRepo.GetAllFavoritesAsync(appUser.Id);  
            if(favorites == null) return NoContent();
            return Ok(favorites); 
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

        [HttpPost("favorite/{ImdbID}")]
        //[Route("{ImdbID}")]
        [Authorize]
        public async Task<IActionResult> CreateFavorite([FromRoute] string ImdbID) //[FromQuery] string title, [FromBody] FavoriteQueryObject queryObject
        {
            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var favoriteDto = new FavoriteDto();            

            var favoriteModel = favoriteDto.ToUserPreferanceFromFavoriteDto();
            favoriteModel.ImdbID = ImdbID;
            favoriteModel.AppUserId = appUser.Id;

            await _userRepo.AddFavoriteAsync(favoriteModel);

            favoriteDto =  await _userRepo.GetFavoriteByFavoriteIdAsync(favoriteModel.Id);

            //return Ok(); //result.UserPreferanceToFavoriteDto()
            return CreatedAtAction(nameof(GetFavoriteByFavoriteId), new { id = favoriteModel.Id}, favoriteDto); //,   favoriteModel.ToFavoriteDto()            
        }

        [HttpDelete("favorite/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFavorite([FromRoute] int id)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var favoriteModel = await _userRepo.DeleteFavoriteAsync(id);
            if(favoriteModel == null) return NotFound("Not succsessfull request.");

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

            return CreatedAtAction(nameof(GetFavoriteByFavoriteId), new { id = followModel.Id}, followDto); //,   favoriteModel.ToFavoriteDto()            
        }

        [HttpDelete("follow/{username}")]
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