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
        public bool IsWatched { get; set; } // true izlendi, default olarak false yani izlenmedi
        public bool IsWatchList { get; set; } // true izleme listemde, default olarak false 
        public DateTime CreatedAt { get; set; } 
        
        //public string ImdbID { get; set; } // OMDb API'den gelen benzersiz film kimliği
        public int MovieId { get; set; }        
        public string AppUserId { get; set; } 
        public AppUser AppUser { get; set; }
    }
}