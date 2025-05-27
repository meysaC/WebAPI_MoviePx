using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService( IConfiguration config, SymmetricSecurityKey key)
        {
            _config = config;
            _key = key; //new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));

            //DI ile SymmetricSecurityKey tanımlayıp kullanınca(Program.cs de) yalnızca bir kez oluşturulur ve uygulama boyunca tekrar kullanılabilir böylece performans artar (çünkü aynı anahtar her yerde yeniden oluşturulmaz.)
            //TokenService nesneleri aynı SymmetricSecurityKey nesnesini kullanır, bu da tutarlılığı sağlar
            //SymmetricSecurityKey test sırasında kolayca mock edilebilir veya değiştirilerek farklı senaryolar test edilebilir
            //DI konteynerine eklenen SymmetricSecurityKey, uygulamanın bağımlılıklarını daha net bir şekilde gösterir.

            //Constructor İçinde SymmetricSecurityKey alnızca bu sınıf içinde kullanılacağı için doğrudan burada oluşturulması daha anlamlı olabilir
            // AMA Her TokenService nesnesi oluşturulduğunda SymmetricSecurityKey yeniden hesaplanır. Bu, performans sorunlarına yol açabilir.
            // test sırasında bu nesneyi taklit etmek daha zor, _config["JWT:SigningKey"] eksik veya hatalıysa, hata yalnızca TokenService oluşturulurken fark edilir
        
        }

        public string CreateToken(AppUser user)
        {
           var claims = new List<Claim>
           {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // token replay saldırılarına karşı
           };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            token.Header.Add("kid", "894626298589");
            return tokenHandler.WriteToken(token);
        }

        public string CreateStateToken() //state i token içerisine gömüyoruz (JWT içine gömülür ve imzalanır)
        {
            //return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var state = Guid.NewGuid().ToString();
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); 
        
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("state", state) }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            token.Header.Add("kid", "894626298589");
            return tokenHandler.WriteToken(token);
        
        }

        public bool ValidateStateToken(string token) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();           
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["JWT:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var state = jwtToken.Claims.First(x => x.Type == "state").Value;

                return !string.IsNullOrEmpty(state);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"State validation failed: {ex.Message}");
                return false;
            }        
        }
    }
}