using Auth.API.Entities;
using Auth.API.Helpers;
using Auth.API.Messaging.Producers;
using Auth.API.Repositories.Behaviours;
using Auth.API.Repositories.Interfaces;
using Auth.API.Services.Behaviours;
using Auth.API.Services.Interfaces;
using AuthService.Repositories.Behaviours;
using AuthService.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using Serilog;
using System.Text;

namespace Auth.API.Configurations
{
    public static class ServicesConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddCors(options =>
            {
                options.AddPolicy("AllowCredentials",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000", "http://103.56.160.183:11114", "http://103.56.160.183:11120", "https://xomcafe.io.vn/")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });



            // Redis configuration
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "13.239.240.248:6379";
                options.InstanceName = "Otp:";
            });

            // DataBase configuration
            var mongoConnectionString = configuration.GetConnectionString("DefaultConnection");
            var mongoClient = new MongoClient(mongoConnectionString);
            var database = mongoClient.GetDatabase("scm-auth");
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton(database);

            // JWT configuration
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

            // Authorize button
            services.AddSwaggerGen(options =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "Jwt",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Enter your JWT Access Token"
                };

                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer")] = []
                });
            });

            services.AddScoped<RedisHelper>();

            // Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add dependency injection
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<EmailHelper>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenService, AuthenService>();
            services.AddScoped<RegisterProducerService>();
            services.AddControllersWithViews();
        }
    }
}