using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StreetService.Core.Interfaces;
using StreetService.Infrastructure.Data;
using StreetService.Infrastructure.Repositories;

namespace StreetService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, x => x.UseNetTopologySuite()));

        services.AddScoped<IStreetRepository, StreetRepository>();
        return services;
    }
}
