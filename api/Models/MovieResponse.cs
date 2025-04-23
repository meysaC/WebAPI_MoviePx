using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class MovieResponse //tmdb api
    {
        public int Page { get; set; }
        public List<Movie> Results { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
    }
}