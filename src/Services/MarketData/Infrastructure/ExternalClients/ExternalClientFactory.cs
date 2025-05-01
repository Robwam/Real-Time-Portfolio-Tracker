using System;
using System.Collections.Generic;
using MarketData.Application.Interfaces;
using MarketData.Models;
using Shared.Models.Enums;
using Microsoft.Extensions.Logging;

namespace MarketData.Infrastructure.ExternalClients;
public class ExternalClientFactory : IExternalClientFactory
{
    private readonly ILogger<ExternalClientFactory> _logger;
    private readonly Dictionary<AssetType, IExternalClient> _clientCache = new();
    private readonly ExternalClientSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalClientFactory(
        ExternalClientSettings settings,
        IHttpClientFactory httpClientFactory,
        ILogger<ExternalClientFactory> logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IExternalClient GetClient(AssetType assetType)
    {
        if (_clientCache.TryGetValue(assetType, out var cachedClient))
            return cachedClient;
            
        var client = CreateClient(assetType);
        _clientCache[assetType] = client;
        return client;
    }
    
    private IExternalClient CreateClient(AssetType assetType)
    {
        return assetType switch
        {
            AssetType.Stock => CreateAlphaVantageClient(assetType),
            //AssetType.Crypto => CreateCoinGeckoClient(assetType),
            _ => throw new ArgumentOutOfRangeException(nameof(assetType))
        };
    }
    
    private AlphaVantageClient CreateAlphaVantageClient(AssetType assetType)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_settings.Providers["AlphaVantage"].BaseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(_settings.Providers["AlphaVantage"].TimeoutSeconds);
        
        return new AlphaVantageClient(httpClient, _settings.Providers["AlphaVantage"].ApiKey, assetType, _logger);
    }
    
    // private CoinGeckoClient CreateCoinGeckoClient(AssetType assetType)
    // {
        
    // }
}