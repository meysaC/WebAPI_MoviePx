using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Mapper;
using api.Dtos.Comment;
using api.Extensions;
using Microsoft.AspNetCore.Identity;


namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IOMDbService _omdbService;
        private readonly ICommentRepository _commentRepo;
        private readonly UserManager<AppUser> _userManager;
        public CommentController(IOMDbService omdbService, ICommentRepository commentRepo, UserManager<AppUser> userManager)
        {
            _omdbService = omdbService;
            _commentRepo = commentRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllMovieComment ([FromQuery] CommentQueryObject queryObject)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var comments = await _commentRepo.GetAllAsync(queryObject);
            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetByCommentId([FromRoute] int id)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var comment = await _commentRepo.GetByIdAsync(id);
            if(comment == null) return NoContent();
            return Ok(comment.ToCommentDto());
        }

        [HttpPost]
        [Route("{ImdbID}")]
        public async Task<IActionResult> Create([FromRoute] string ImdbID,[FromBody] CommentCreateDto commentDto) //ImdbID mi filmin title mı????????????????????
        {
            var movie = await _omdbService.GetMovieByIdAsync(ImdbID);
            if(movie == null) return BadRequest("The movie doesn't exists.");

            var userName = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(userName);

            var commentModel = commentDto.ToCommentFromCreateDto(ImdbID);
            commentModel.AppUserId = appUser.Id;
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetByCommentId), new { id = commentModel.Id}, commentModel.ToCommentDto());
        }
    
        [HttpPut]
        [Authorize]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CommentUpdateDto commentDto)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var comment = await _commentRepo.UpdateAsync(id, commentDto.ToCommentFromUpdateDto());
            if(comment == null) return BadRequest("Comment not found.");



            return Ok(comment.ToCommentDto());
        }
    
        [HttpDelete]
        [Authorize]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid ) return BadRequest(ModelState);

            var commentModel = await _commentRepo.DeleteAsync(id);
            if(commentModel == null) return NotFound("Comment doesn't exists.");

            return NoContent();
        }
    }
}