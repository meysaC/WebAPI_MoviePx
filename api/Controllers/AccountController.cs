using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
        //private readonly IRedisCacheService _redisCacheService;
        private readonly IOAuthService _oauthService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signinManager,ITokenService tokenService, IOAuthService oauthService)//, IRedisCacheService redisCacheService
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
            //_redisCacheService = redisCacheService;
            _oauthService = oauthService;
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
                    id = user.Id,
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
                            id = appUser.Id,
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

        [HttpGet("google_login_url")] //HttpPost
        public async Task<IActionResult> GoogleLoginUrl() 
        {
            try
            {
                var stateToken = _tokenService.CreateStateToken();
                var loginUrl = _oauthService.GetGoogleLoginUrl(stateToken); //stateToken
                //return Redirect(loginUrl);
                return Ok(new { url = loginUrl, stateToken = stateToken });  //Google Login URL'sini frontend'e döndürecek. Frontend bu URL'ye yönlendirerek Google Login işlemini başlatacak.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google Login Error: {ex.Message}");
                return StatusCode(500, "Google Login Error");
            }
        }
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string code, string state) //state güvenlik sağlar(token saldırılara karşı(CSRF (Cross-Site Request Forgery)))
                                                                                //code, Google'dan code kullanılarak bir erişim token (kullanıcı bilgileri  için gerekli yetki)
        {            
            try
            {                
                //GoogleOptions kullanıyorsan, state kontrolü Google middleware tarafından otomatik yapılır. Kendi ValidateStateToken metodunu çağırman çakışmaya veya double validation’a neden olabilir.
                // if (!_tokenService.ValidateStateToken(state))
                // {
                //     return Unauthorized("Invalid state token");
                // }
                var googleUser = await _oauthService.GetGoogleUserInfoAsync(code, state); 
                if (googleUser == null) return StatusCode(500, "Google authentication failed.");

                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //Bazı Identity konfigürasyonlarında FindByEmailAsync() case-sensitive çalışabilir. NormalizedEmail ile sorgulamak, email doğrulama hatalarını engeller.
                // var normalizedEmail = googleUser.Email.ToUpper();
                // var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

                var appUser = await _userManager.FindByEmailAsync(googleUser.Email);
                if (appUser == null)
                {
                    // var normalizedEmail = googleUser.Email.ToUpper();
                    // var normalizedUserName = googleUser.Name.ToUpper();
                    appUser = new AppUser
                    {
                        UserName = googleUser.Email,
                        Email = googleUser.Email,
                        
                        // EmailConfirmed = true, //// Google doğrulama yaptığı için true olabilir                        
                        // NormalizedUserName = normalizedUserName,
                        // NormalizedEmail = normalizedEmail,
                    };

                    var createdUser = await _userManager.CreateAsync(appUser);
                    if (!createdUser.Succeeded)
                    {
                        foreach (var error in createdUser.Errors)
                        {
                            Console.WriteLine($"Error Code: {error.Code}, Description: {error.Description}");
                        }                    
                        return StatusCode(500, createdUser.Errors);
                    }
                }

                var token = _tokenService.CreateToken(appUser);
                return Ok(new
                {
                    id = appUser.Id,
                    UserName = appUser.UserName,//Email,
                    Email = appUser.Email,
                    Token = token
                });
                // return Ok(appUser);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google Callback Error: {ex.Message}");
                return StatusCode(500, "An error occurred during Google callback.");
            }

        }
    }
}