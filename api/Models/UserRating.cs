using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("UserRatings")]
    public class UserRating
    {
        public int Id { get; set; }       
        public double Rate { get; set; }

        public string ImdbID { get; set; }
        public string AppUserId { get; set; } 
        public AppUser AppUser { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now; //burda eşitlikle mi tanımlamalyız????????

    }
}