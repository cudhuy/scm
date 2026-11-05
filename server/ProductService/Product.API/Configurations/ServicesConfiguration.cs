using CloudinaryDotNet;
using Domain.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Product.API.Helpers;
using Product.API.Repositories.Behaviours;
using Product.API.Repositories.Interfaces;
using Product.API.Services.Behaviours;
using Product.API.Services.Interfaces;
using Serilog;
using System.Text;

namespace Product.API.Configurations
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
                        builder.WithOrigins("http://localhost:3000", "http://103.56.160.183:11117", "http://103.56.160.183:11120", "https://xomcafe.io.vn/")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            // Cloundinary
            services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                var cloudName = configuration["CloudinarySettings:CloudName"];
                var apiKey = configuration["CloudinarySettings:ApiKey"];
                var apiSecret = configuration["CloudinarySettings:ApiSecret"];

                var account = new Account(cloudName, apiKey, apiSecret);
                return new Cloudinary(account);
            });

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




            // DataBase configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<SCMDbContext>(options =>
                options.UseMySQL(connectionString));

            // Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IIngredientsRepository, IngredientsRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();

            // Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IIngredientsService, IngredientsService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<EmailHelper>();
            services.AddScoped<CloudinaryHelper>();

            // Đăng ký Background Service
            services.AddHostedService<AlertBackgroundService>();
        }
    }
}
