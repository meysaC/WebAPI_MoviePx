using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class AppUser : IdentityUser
    {
        // public List<UserPreferance> UserPreferances { get; set; } = new List<UserPreferance>();
        // public List<Comment> Comments { get; set; } = new List<Comment>();
        // public List<UserRating> UserRatings { get; set; } = new List<UserRating>();
        
        public string? ProfileImageUrl { get; set; }
        public string? BannerImageUrl { get; set; }

        public ICollection<UserFollow> Followings { get; set; } //Takip ettiklerim
        public ICollection<UserFollow> Followers { get; set; } // takip edenler
    }
}