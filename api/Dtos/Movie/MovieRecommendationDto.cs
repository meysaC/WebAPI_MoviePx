using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace api.Dtos.Movie
{
    public class MovieRecommendationDto
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty; // Kullanıcının duygu/kategori tarif metni 
    }
}