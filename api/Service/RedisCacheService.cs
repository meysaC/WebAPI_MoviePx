using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace api.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _cacheDb;
        private readonly int _defaultExpiryMinutes;
        private readonly IConfiguration config;
       
       
       //Redis ile bağlantıyı ConnectionMultiplexer sınıfı üzerinden sağlıyoruz. Bu sınıf Redis ile bağlantıyı yöneten ve birden fazla bağlantıyı yeniden kullanabilen bir sınıftır.
       //Lazy<T> , bağlantının yalnızca ilk kullanımda yapılmasını sağlar, performans açısından verimli bir çözümdür. Özellikle bağlantının pahalı bir işlem olduğu durumlarda yararlıdır. birden çok iş parçacığının (thread) aynı anda bağlantıyı başlatmaya çalışmasını engelleyecek şekilde tasarlanmıştır. Bu sayede, bağlantıyı yalnızca ilk erişimde başlatır ve başka bir iş parçacığının bağlantıyı tekrar başlatmasını engeller.
       //statik olması, uygulama başlatıldığında sınıfın bir örneği oluşturulmadan önce Redis bağlantısının tembel bir şekilde başlatılmasını sağlar.Redis bağlantısının tek bir yerde ve yalnızca bir kez yapılmasını garanti eder.
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection;
        private static ConnectionMultiplexer Connection => LazyConnection.Value; //nesnesinin Value özelliğini çağırır. Bu özellik, nesneye ilk erişimde bağlantıyı kurar.Eğer bağlantı daha önce yapılmışsa, Lazy nesnesi zaten oluşturulan bağlantıyı döndürür
       
       // Statik constructor  (BU SERVİSE GELİNDİĞİNDE BU CONSTRUCTOR SADECE BİR KEZ ÇALIŞIR VE BİR DAHA BU SERVİSE GELİNDİĞİNDE BU SINIFA BİR DAHA GİRMEZ)
        static RedisCacheService()
        {
            LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                return ConnectionMultiplexer.Connect(config["RedisSettings:ConnectionString"]);
            });  
        }
        
        // Yapıcı metot
        public RedisCacheService(IConfiguration configuration) 
        {
            //var redis = ConnectionMultiplexer.Connect(config["RedisSettings:ConnectionString"]);
            config = configuration;
            _cacheDb = Connection.GetDatabase(); //redis
            _defaultExpiryMinutes = int.Parse(config["RedisSettings:DefaultExpiryMinutes"]);
        }



        public async Task<T> GetCacheAsync<T>(string key)
        {
            try
            {
                var value = await _cacheDb.StringGetAsync(key); 
                //if(!string.IsNullOrEmpty(value))    //bunun yerine alttaki gibi tek satır kodunda yaz!!  o zaman return default demene de gerek yok      
                return value.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<T>(value); 

                //return default;
            }
            catch (Exception ex)
            {
                throw new Exception($"Redis GetCacheAsync Error: {ex.Message}", ex);
            }

        }

        public async Task<bool> SetCacheAsync<T>(string key, T value) //, DateTimeOffset expirationTime
        {
            try
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(_defaultExpiryMinutes); 
                //var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
                var expiryTime = TimeSpan.FromMinutes(_defaultExpiryMinutes);
                
                var isSet = await _cacheDb.StringSetAsync(key, JsonConvert.SerializeObject(value), expiryTime); //JsonSerializer.Serialize(value)
                return isSet;
            }
            catch (Exception ex)
            {
                
                throw new Exception($"Redis SetCacheAsync Error: {ex.Message}", ex);
            }
        }

        public async Task<object> RemoveCacheAsync(string key) //Task
        {
            try
            {
                //var _exists = _cacheDb.KeyExists(key);
                return _cacheDb.KeyDeleteAsync(key); //if(_exists) 
            }
            catch (Exception ex)
            {
                throw new Exception($"Redis RemoveCacheAsync Error: {ex.Message}", ex);
            }
        }

    }    
}