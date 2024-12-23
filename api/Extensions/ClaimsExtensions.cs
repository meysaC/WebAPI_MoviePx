using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static  class ClaimsExtensions
    {
        public static string GetUsername (this ClaimsPrincipal user) //we are gonna reach into the claims with user object (we get our claims from tokens !!)
        {
            var claim = user?.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"));
            return claim?.Value ?? throw new InvalidOperationException("Given name claim not found.");
            //return user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value; //there is already a method to reach into a claim (this not a url its a uri)
        }

    }
}