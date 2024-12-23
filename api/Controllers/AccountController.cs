using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly ITokenService _tokenService;
        private readonly IAccountrepository _accountRepo;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signinManager,ITokenService tokenService,  IAccountrepository accountRepo)//
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
            _accountRepo = accountRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto) //
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
            if(User == null) return Unauthorized("Invalid username!");

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if(!result.Succeeded) return Unauthorized("Username not found or pasword incorret.");

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Token = _tokenService.CreateToken(user)
                }
            );
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest(ModelState);

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
                if(createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if(roleResult.Succeeded) return Ok(
                        new NewUserDto
                        {
                            UserName = appUser.UserName ?? "",
                            Email = appUser.Email ?? "",
                            Token = _tokenService.CreateToken(appUser)
                        }
                    );
                    else return StatusCode(500, roleResult.Errors);
                }
                else 
                {
                    return StatusCode(500, createdUser.Errors);
                }


            }
            catch(Exception e)
            {
                return StatusCode(500, e);   
            }
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetAllFavorite()  //ASLINDA BU TÜM KULLANICILARIN DİĞER KULLANICILARIN FAVORİLERİNE BAKMASI OLMALI O YÜZDEN APPUSER.ID DEĞİL DE KİME TIKLANDIYA ONUN İD Sİ GİTMELİ
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var favorites = await _accountRepo.GetAllFavoritesAsync(appUser.Id);  
            if(favorites == null) return NoContent();
            return Ok(favorites); 
        }
        
        [HttpGet("favorite/{id:int}")]
        //[Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteByFavoriteId([FromRoute] int id )
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var favorite = await _accountRepo.GetFavoriteByFavoriteIdAsync(id);  
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

            await _accountRepo.AddFavoriteAsync(favoriteModel);

            favoriteDto =  await _accountRepo.GetFavoriteByFavoriteIdAsync(favoriteModel.Id);

            //return Ok(); //result.UserPreferanceToFavoriteDto()
            return CreatedAtAction(nameof(GetFavoriteByFavoriteId), new { id = favoriteModel.Id}, favoriteDto); //,   favoriteModel.ToFavoriteDto()            
        }


        [HttpDelete("favorite/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFavorite([FromRoute] int id)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var favoriteModel = await _accountRepo.DeleteFavoriteAsync(id);
            if(favoriteModel == null) return NotFound("Not succsessfull request.");

            return NoContent();
        }
    }
}