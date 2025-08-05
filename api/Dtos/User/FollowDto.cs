using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class FollowDto
    {
        public int Id { get; set; }
        // takip eden
        public string FollowerId { get; set; }
        public string FollowerUserName { get; set; } //sadece ıd döndürmek yetmez frontend de tekrar GetUserById çağırmak zorunda kalmamak için
        // public string FollowerEmail { get; set; }

        // takip edilen
        public string FollowingId { get; set; }
        public string FollowingUserName { get; set; }
        // public string FollowingEmail { get; set; }

        public bool IsActive { get; set; }
        public DateTime FollowedWhen { get; set; }
        public DateTime? UnFollowedWhen { get; set; }
    }
}