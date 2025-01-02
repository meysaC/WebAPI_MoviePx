using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Newtonsoft.Json;

namespace api.Service
{
    public class OAuthService : IOAuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenSer; 
        public OAuthService(IConfiguration config, HttpClient httpClient, ITokenService tokenSer)
        {
            _config = config;
            _httpClient = httpClient;
            _tokenSer = tokenSer;
        }
        public string GetGoogleLoginUrl()
        {
            var state = _tokenSer.CreateStateToken();
            var scope = Uri.EscapeDataString("email profile");
            
            var googleAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                    $"client_id={_config["GoogleOAuth:ClientId"]}&" +   //Hangi uygulamanın oturum açma talebinde bulunduğu
                    $"redirect_uri={Uri.EscapeDataString(_config["GoogleOAuth:RedirectUri"])}&" + // Google oturum açma işlemi tamamlandığında yönlendirme yapılacak URL
                    $"response_type=code&" + //Yanıt türünü (code) belirler (web uyg code yazılır)
                    $"scope={scope}&" + //Hangi bilgilere erişileceği
                    $"state={Uri.EscapeDataString(state)}";  //güvenlik için token 
            return googleAuthUrl;
        }

        public async Task<GoogleUserInfo?> GetGoogleUserInfoAsync(string code, string state)
        {
            // access token Al (oauth Token bazlı yetkilendirme olayını access token ve refresh token olarak ikiye ayırmışlar)  (Tıpkı bir API anahtarı gibi gönderilebilen, uygulamaya erişim sağlayan tokendır) ()
            var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"code", code}, // google oturum açma işleminden dönen yetkilendirme kodu
                    {"client_id", _config["GoogleOAuth:ClientId"]}, 
                    {"client_secret", _config["GoogleOAuth:ClientSecret"]}, //google api ye erişim sağlamak için gereken gizli anahtar
                    {"redirect_uri", _config["GoogleOAuth:RedirectUri"]}, //google ın yanıtı göndereceği URL
                    {"grant_type", "authorization_code"} //  Bu işlem için her zaman authorization_code kullanılır (google dan authorization_code kullanarak access token alınır) (acces ve refresh tokenları güvenilir kullanıcılar için optimize edilmiş onay tipi)
                }));
            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Error getting token: " + errorContent);
                return null;
            } 

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonConvert.DeserializeObject<Credentials>(tokenJson); 
            if (string.IsNullOrEmpty(tokenData?.access_token))
            {
                Console.WriteLine("Access token is empty or null.");
                Console.WriteLine("Token response: " + tokenJson);
                return null;
            }

            //kullanıcı bilgileri
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.access_token); //kullanıcı bilgileri için Bearer token (HTTP başlıklarında taşındıkları için URL'lerde görünmez)(api token ile her isteği bağımsız olarak doğrular (stateless) sunucu tarafında oturum bilgisi saklanmadığı için uygulamanın ölçeklenmesi daha kolay) ile kimlik doğrulama 
                                                                                                                                //OAuth 2.0 ile birlikte gelen bu özellik korunaklı kaynaklara HTTP ile erişim isteğinde bulunurken Authorization başlığı içerisinde ‘Bearer’ anahtar kelimesinin kullanılmasına imkan veren yapı
                                                                                                                                //Burada amaç istemcinin kendini kanıtlamak için kriptografik bir anahtarı olmadan kendini doğrulayabilmesi için kullanılan yöntem
                                                                                                                                //Bearer sözcüğünden sonra kullanılacak token’ın dışa açık olmaması önemlidir (Bu yöntem sayesinde OpenID Connect ile gelişmiş tokenlar üretebilir ve kullanılabilir alt yapı sağlanmakta)
                                                                                                                                //(httpclient-> tekrar tekrar yaratılmadan api çağrıları için kullanılabilir)
            var userInfoResponse = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo"); //Google ın kullanıcı bilgilerini sağlayan API endpoint i
            if (!userInfoResponse.IsSuccessStatusCode)
            {
                var userInfoError = await userInfoResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Error getting user info: " + userInfoError);
                return null;
            } 

            var userJson = await userInfoResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleUserInfo>(userJson);     // User JsonSerializer.Deserialize 
        }

    }
}