using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface IOAuthService
    {
        string GetGoogleLoginUrl(string state);
        Task<GoogleUserInfo?> GetGoogleUserInfoAsync(string code, string state); //
    }
}