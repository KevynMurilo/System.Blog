using Microsoft.EntityFrameworkCore;
using System.Blog.Infrastructure.Data;

namespace System.Blog.Web.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        return services;
    }
}
