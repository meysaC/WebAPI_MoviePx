using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class FollowDto
    {
        public int Id { get; set; }
        public string userName { get; set; } = string.Empty;
        public string FollowingUserName { get; set; } = string.Empty;
        public string Since { get; set; }        
    }
}