using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("UserPreferances")]
    public class UserPreferance
    {
        public int Id { get; set; } 
        public bool IsFavorite { get; set; } // true favori, false dislike
        public DateTime CreatedAt { get; set; } 
        
        public string ImdbID { get; set; } // OMDb API'den gelen benzersiz film kimliği
        public string AppUserId { get; set; } 
        public AppUser AppUser { get; set; }

    }
}