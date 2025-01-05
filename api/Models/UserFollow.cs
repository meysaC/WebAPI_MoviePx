using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("UserFollows")]
    public class UserFollow
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } 
        public AppUser AppUser { get; set; }
        public string FollowingId { get; set; }
        public DateTime FollowedWhen { get; set;} = DateTime.Now;
    }
}