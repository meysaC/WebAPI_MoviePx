using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Movie;

namespace api.Dtos.User
{
    public class WatchedDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string AppUserId { get; set; }
        public MovieDto? Movie { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}