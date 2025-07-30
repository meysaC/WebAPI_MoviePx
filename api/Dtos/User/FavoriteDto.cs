using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Movie;

namespace api.Dtos.User
{
    public class FavoriteDto
    {
        //public int Id { get; set; }
        //public string? Title { get; set; }
        //public string? Director { get; set; }
        //public string? imdbRating { get; set; }
        //public string Favorite { get; set; } = "Favorite"; 

        //public Models.Movie Movie { get; set; }

        public int Id { get; set; }
        public int MovieId { get; set; }
        public string AppUserId { get; set; }
        public MovieDto? Movie { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}