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
        
        var externalSettings = new ExternalClientSettings();
        configuration.GetSection("MarketData:External").Bind(externalSettings);
        services.AddSingleton(externalSettings);
        
        // Register Redis connection
        var redisConnectionString = configuration.GetConnectionString("Redis");
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(redisConnectionString ?? "localhost:6379"));
        
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