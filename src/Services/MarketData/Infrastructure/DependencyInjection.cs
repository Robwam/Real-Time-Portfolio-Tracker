using MarketData.Application.Interfaces;
using MarketData.Infrastructure.Cache;
using MarketData.Application.Services;
using MarketData.Services;
using MarketData.Infrastructure.ExternalClients;
using MarketData.Models;
using StackExchange.Redis;

namespace MarketData.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddMarketDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register settings from configuration
        var cacheSettings = new CacheSettings();
        configuration.GetSection("MarketData:Cache").Bind(cacheSettings);
        services.AddSingleton(cacheSettings);
        
        var externalClientSettings = new ExternalClientSettings();
        configuration.GetSection("MarketData:ExternalClients").Bind(externalClientSettings);
        services.AddSingleton(externalClientSettings);

        // Register Redis connection
        var redisConnectionString = configuration["Redis:ConnectionString"];
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));
        
        // Register HTTP clients
        services.AddHttpClient();
        
        // Register services
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IMarketDataCache, MarketDataCache>();
        services.AddSingleton<IExternalClientFactory, ExternalClientFactory>();
        services.AddScoped<IMarketDataService, MarketDataService>();
        services.AddSingleton<CacheKeyManager, CacheKeyManager>();
        
        return services;
    }
}