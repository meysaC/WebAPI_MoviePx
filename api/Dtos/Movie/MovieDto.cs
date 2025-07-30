using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace api.Dtos.Movie
{
    public class MovieDto //FRONT A GÖNDERMEK İSTEDİĞİMİZ VERİLERİ GÖNDERİYORUZ SADECE!!
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }
        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
    }
}