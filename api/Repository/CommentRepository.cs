using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;

        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetAllAsync(CommentQueryObject queryObject)
        {
           var comments = _context.Comments.Include(a => a.AppUser).AsQueryable();
           if(queryObject.MovieId.HasValue ) //!string.IsNullOrEmpty
           {
                comments = comments.Where(s => s.MovieId == queryObject.MovieId);
           }
           else if(!string.IsNullOrEmpty(queryObject.Title) )
           {
                comments = comments.Where(s => s.Title == queryObject.Title);
           }
          
           if(queryObject.IsDecsending)
           {
            comments = comments.OrderByDescending(c => c.CreatedOn);
           }
           
           return await comments.ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
        }
        
        public async Task<Comment?> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }
        
        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
           var existingComment = await _context.Comments.Include(c => c.AppUser).FirstOrDefaultAsync(a => a.Id == id);
           if(existingComment == null) return null;

           existingComment.Title = commentModel.Title ?? existingComment.Title;
           existingComment.Content = commentModel.Content;
           existingComment.UpdatedOn = DateTime.Now;

           await _context.SaveChangesAsync();
           return existingComment;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(a => a.Id == id);
            if(commentModel == null) return null;
            _context.Remove(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }
    }
}