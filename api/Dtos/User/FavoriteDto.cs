using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class FavoriteDto
    {
        public int Id { get; set; }
        public string? Title {get; set;}
        public string? Director { get; set; }
        public string? imdbRating { get; set; }
        public string Favorite { get; set; } = "Favorite";
    }
}