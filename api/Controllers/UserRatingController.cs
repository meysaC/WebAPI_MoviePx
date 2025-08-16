using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.UserRating;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mapper;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/UserRatings")]
    [ApiController]
    public class UserRatingController : ControllerBase
    {
        private readonly IUserRatingRepository _ratingRepo;
        private readonly UserManager<AppUser> _userManager;
        public UserRatingController(IUserRatingRepository ratingRepo, UserManager<AppUser> userManager)
        {
            _ratingRepo = ratingRepo;
            _userManager = userManager;
        }
        
        [HttpGet("rating/{id:int}")]
        //[Route("{id:int}")]
        public async Task<IActionResult> GetRatingByById([FromRoute] int id)
        {
            var rating = await _ratingRepo.GetRatingByIdAsync(id);
            return Ok(rating.UserRatingToUserRatingDto());
        }
        
        [HttpGet("User_s_Ratings")]
        public async Task<IActionResult> GetAllRatingsForUser()
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var ratings = await _ratingRepo.GetAllRatingsForUserAsync(appUser.Id);
            if(ratings == null) return NotFound("Doesn't exists.");
            
            var ratingsDto = ratings.Select(a => a.UserRatingToUserRatingDto()).ToList();
            return Ok(ratingsDto);
        }

        [HttpGet("ratings_for_movie/{MovieId}")]
        //[Route("{MovieId}")]
        public async Task<IActionResult> GetAllRatingsByMovieId([FromRoute] int MovieId)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var ratings = await _ratingRepo.GetAllRatingsByMovieIdAsync(MovieId);

            var ratingsDto = ratings.Select(a => a.UserRatingToUserRatingDto()).ToList();
            return Ok(ratingsDto);
        }

        [HttpGet("rating_ratio_for_movie/{MovieId}")]
        //[Route("{MovieId}")]
        public async Task<IActionResult> GetRatingRatioByMovieId([FromRoute] int MovieId)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var ratio = await _ratingRepo.GetRatingRatioByMovieIdAsync(MovieId);
            var result = api.Mapper.UserRatingMapper.UserRatingToMovieRatingRatioDto(MovieId, ratio);
            return Ok(result);
        }

        [HttpPost("create_rating")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateUserRatingDto ratingDto) 
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var userRatingModel = ratingDto.CreateUserRatingDtoToUserRating();
            userRatingModel.AppUser = appUser;  //??????????????

            await _ratingRepo.CreateAsync(userRatingModel);
            return CreatedAtAction(nameof(GetRatingByById), new { id = userRatingModel.Id}, userRatingModel.UserRatingToUserRatingDto());
        }

        [HttpPut("rating_update/{id}/{rate}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromRoute] double rate)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var ratingModel = await _ratingRepo.UpdateAsync(id, rate);

            return CreatedAtAction(nameof(GetRatingByById), new { id = ratingModel.Id}, ratingModel.UserRatingToUserRatingDto());
        }
    
        [HttpDelete("rating_delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var ratingModel = await _ratingRepo.DeleteAsync(id);
            if(ratingModel == null) return NotFound("Rating doesn't exists.");

            return NoContent();
        }
    }
}