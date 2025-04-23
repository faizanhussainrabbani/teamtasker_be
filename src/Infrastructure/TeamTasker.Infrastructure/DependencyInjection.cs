using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Authentication;
using TeamTasker.Infrastructure.Data;
using TeamTasker.Infrastructure.Repositories;
using TeamTasker.Infrastructure.Services;
using TeamTasker.Infrastructure.Settings;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Infrastructure
{
    /// <summary>
    /// Extension methods for setting up infrastructure services in an IServiceCollection
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Register services
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Register JWT services
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            // Register email and password reset services
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IPasswordResetTokenService, PasswordResetTokenService>();

            // Register distributed cache for token storage
            services.AddDistributedMemoryCache();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

            return services;
        }
    }
}
