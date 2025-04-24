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
        try
        {
            var providerName = GetProviderName(assetType);
            return providerName.ToLowerInvariant() switch
            {
                "alphavantage" => CreateAlphaVantageClient(assetType),
                //"coingecko" => CreateCoinGeckoClient(assetType),
                _ => throw new NotSupportedException($"Provider {providerName} is not supported")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client for asset type {AssetType}, trying fallback", assetType);
            throw new InvalidOperationException($"Failed to create client for asset type {assetType}", ex);
        }
    }
    
    private string GetProviderName(AssetType assetType)
    {
        return assetType switch
        {
            AssetType.Stock => _settings.StockProvider,
            AssetType.Crypto => _settings.CryptoProvider,
            _ => throw new ArgumentOutOfRangeException(nameof(assetType))
        };
    }
    
    private AlphaVantageClient CreateAlphaVantageClient(AssetType assetType)
    {
        var httpClient = _httpClientFactory.CreateClient("AlphaVantage");
        httpClient.BaseAddress = new Uri("https://www.alphavantage.co/");
        
        return new AlphaVantageClient(httpClient, _settings.AlphaVantageApiKey, assetType, _logger);
    }
    
    // private CoinGeckoClient CreateCoinGeckoClient(AssetType assetType)
    // {
        
    // }
}

public class ExternalClientSettings
{
    public string StockProvider { get; set; } = "AlphaVantage";
    public string CryptoProvider { get; set; } = "CoinGecko";
    
    public string AlphaVantageApiKey { get; set; }
    public string CoinGeckoApiKey { get; set; }
}