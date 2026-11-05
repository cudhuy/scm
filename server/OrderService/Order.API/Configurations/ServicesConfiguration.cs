using Domain.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Order.API.Repositories.Behaviours;
using Order.API.Repositories.Interfaces;
using Order.API.Services.Behaviours;
using Order.API.Services.Interfaces;
using Serilog;
using System.Text;

namespace Order.API.Configurations
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
                        builder.WithOrigins("http://localhost:3000", "http://103.56.160.183:11118", "http://103.56.160.183:11120", "https://xomcafe.io.vn", "http://127.0.0.1:5500")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
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
            services.AddSignalR();

            // DataBase configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<SCMDbContext>(options =>
                options.UseMySQL(connectionString));

            // Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentsRepository, PaymentsRepository>();

            // Services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentsService, PaymentsService>();
        }
    }
}
