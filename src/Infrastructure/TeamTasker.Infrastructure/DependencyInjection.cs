using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Data;
using TeamTasker.Infrastructure.Repositories;
using TeamTasker.Infrastructure.Services;
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
                options.UseSqlServer(
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

            return services;
        }
    }
}
