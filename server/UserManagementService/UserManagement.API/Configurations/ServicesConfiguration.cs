using CloudinaryDotNet;
using Domain.Context;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using UserManagement.API.Helpers;
using UserManagement.API.Messaging.Producers;
using UserManagement.API.Repositories.Behaviours;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;
using UserManagement.API.Services.Interfaces;
using UserManagement.API.Validators;

namespace UserManagement.API.Configurations
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
                        builder.WithOrigins("http://localhost:3000", "http://103.56.160.183:11113", "http://103.56.160.183:11120", "https://xomcafe.io.vn", "http://127.0.0.1:5500")
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
                    Description = "Enter your JWT Access Token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            // Redis configuration
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "13.239.240.248:6379";
                options.InstanceName = "Otp:";
            });

            // DataBase configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<SCMDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 33))));

            services.AddScoped<RedisHelper>();
            services.AddValidatorsFromAssemblyContaining<CreateMonthlySalaryDtoValidator>();

            // Auto Mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Add dependency injection
            services.AddScoped<CloudinaryHelper>();
            services.AddScoped<EmailHelper>();
            services.AddScoped<CreateStaffProducerService>();
            services.AddScoped<SendSalarySlipProducerService>();
            services.AddScoped<SendNotificationEmailProducerService>();
            services.AddScoped<UpdateProfileProducerService>();
            // Repositories
            services.AddScoped<ICustomerFeedbacksRepository, CustomerFeedbacksRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
            services.AddScoped<IEmployeeShiftsRepository, EmployeeShiftsRepository>();
            services.AddScoped<IMembershipRanksRepository, MembershipRanksRepository>();
            services.AddScoped<INotificationsRepository, NotificationsRepository>();
            services.AddScoped<ISalaryRepository, SalaryRepository>();
            services.AddScoped<IStaffInformationRepository, StaffInformationRepository>();
            services.AddScoped<IUserRolesRepository, UserRolesRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IWorkShiftsRepository, WorkShiftsRepository>();
            // Services
            services.AddScoped<ICustomerFeedbacksService, CustomerFeedbacksService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEmployeeAttendanceService, EmployeeAttendanceService>();
            services.AddScoped<IEmployeeShiftsService, EmployeeShiftsService>();
            services.AddScoped<IMembershipRanksService, MembershipRanksService>();
            services.AddScoped<INotificationsService, NotificationsService>();
            services.AddScoped<ISalaryService, SalaryService>();
            services.AddScoped<IStaffInformationService, StaffInformationService>();
            services.AddScoped<IUserRolesService, UserRolesService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IWorkShiftsService, WorkShiftsService>();
            services.AddScoped<IRankUpdateService, RankUpdateService>();
            services.AddHostedService<RankBackgroundService>();
            services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceProvider>().GetService<IServiceScopeFactory>());


        }
    }
}
