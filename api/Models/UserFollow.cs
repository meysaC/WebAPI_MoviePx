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

        public string FollowerId { get; set; } //takip eden 
        public AppUser Follower { get; set; }

        public string FollowingId { get; set; } //takip edilen
        public AppUser Following { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime FollowedWhen { get; set; } = DateTime.UtcNow;
        public DateTime? UnFollowedWhen { get; set; } 
        
    }
}