using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user); 
        string CreateStateToken(); //oauth için!!!!!!!!!!!1
        bool ValidateStateToken(string token); //oauth için!!!!!!!!!!!1
    }
}