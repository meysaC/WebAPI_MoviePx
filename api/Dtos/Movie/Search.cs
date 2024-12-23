using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Movie
{
    public class Search
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Genre { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
    }
}