using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Movie
{
    public class SearchResult
    {
        public List<Search> Search { get; set; } //json to poco diyince çıkan özelliklerin isimlerini değiştirme!!! 
                                                //Search olmalı mesela burası SearchResult ismini değiştirebilirsin ama 
        public string? totalResults { get; set; }
        public string? Response { get; set; }
    }
}