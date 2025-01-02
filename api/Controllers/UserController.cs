using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
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
        //[Route("{id:int}")]
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

    }
}