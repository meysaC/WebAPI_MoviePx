using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Credentials //https://www.googleapis.com/oauth2/v2/userinfo get ile dönen json formatındaki verinin özellikleri bu şekilde olduğu için (json to poco)
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }

    }
}