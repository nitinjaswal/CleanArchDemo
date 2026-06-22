using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register SQL Server with connection string from appsettings.json
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // THIS IS THE KEY LINE your interviewer asked about!
        // IUserRepository (Domain) → UserRepository (Infrastructure)
        // At runtime .NET knows: when someone asks for IUserRepository, give them UserRepository
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}