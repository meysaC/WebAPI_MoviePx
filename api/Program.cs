using System.Text;
using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//REDİS
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http, //ApiKey
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    //SONRADAN!!!!!!!!!!!!!!!
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddDbContext<ApplicationDBContext>(options => {
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
   options.Password.RequireDigit = true;
   options.Password.RequireLowercase = true;
   options.Password.RequireUppercase = true;
   options.Password.RequireNonAlphanumeric = true;
 options.Password.RequiredLength = 12;
}).AddEntityFrameworkStores<ApplicationDBContext>();


builder.Services.AddAuthentication(options => {
   options.DefaultAuthenticateScheme = 
   options.DefaultChallengeScheme = 
   options.DefaultForbidScheme =
   options.DefaultScheme = 
   options.DefaultSignInScheme = 
   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
   options.TokenValidationParameters = new TokenValidationParameters
   {
      ValidateIssuer = true,
      ValidIssuer = builder.Configuration["JWT:Issuer"],
      ValidateAudience = true,
      ValidAudience = builder.Configuration["JWT:Audience"],
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(
         System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
      ),
      ValidateLifetime = true
   };

   options.Events = new JwtBearerEvents
   {
      OnAuthenticationFailed = context =>
      {
         Console.WriteLine($"Authentication failed: {context.Exception.Message}");
         return Task.CompletedTask;
      },
      OnTokenValidated = context =>
      {
         Console.WriteLine("Token successfully validated.");
         return Task.CompletedTask;
      }
   };    

});

//DI ile SymmetricSecurityKey sağlıyoruz (yalnızca bir kez oluşturulur ve uygulama boyunca tekrar kullanılabilir)
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]));
builder.Services.AddSingleton(signingKey);

builder.Services.AddScoped<IOMDbService, OMDbService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpClient<IOMDbService, OMDbService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IAccountrepository, AccountRepository>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>(); //AddSingleton
builder.Services.AddScoped<IUserRatingRepository, UserRatingRepository>(); 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(options => 
   {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
      options.RoutePrefix = string.Empty;
   });
}

app.UseHttpsRedirection();

app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials() 
        .SetIsOriginAllowed(origin => true)
);


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

