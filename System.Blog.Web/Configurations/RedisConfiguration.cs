using StackExchange.Redis;
using System.Blog.Core.Contracts.Services;
using System.Blog.Infrastructure.Services;

namespace System.Blog.Web.Configurations;

public static class RedisConfiguration
{
    public static IServiceCollection AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("RedisConnection");

        var multiplexer = ConnectionMultiplexer.Connect(connectionString);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddScoped<IRedisService, RedisService>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
        });

        return services;
    }
}
