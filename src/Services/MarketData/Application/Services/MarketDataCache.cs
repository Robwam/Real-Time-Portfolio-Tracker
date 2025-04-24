using MarketData.Application.Interfaces;
using MarketData.Models;
using Shared.Models.DTOs;
using Shared.Models.Enums;

namespace MarketData.Services;
public class MarketDataCache : IMarketDataCache
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<MarketDataCache> _logger;
    private readonly CacheKeyManager _cacheManager;

    public MarketDataCache(
        ICacheService cacheService,
        CacheKeyManager manager,
        ILogger<MarketDataCache> logger)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _cacheManager = manager ?? throw new ArgumentNullException(nameof(manager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AssetPrice> GetPriceAsync(string symbol, AssetType assetType, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = _cacheManager.GeneratePriceKey(symbol, assetType);
            return await _cacheService.GetAsync<AssetPrice>(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving price from cache for {Symbol} ({AssetType})", symbol, assetType);
            return null;
        }
    }

    public async Task CachePriceAsync(AssetPrice price, AssetType assetType, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = _cacheManager.GeneratePriceKey(price.Symbol, assetType);
            var ttl = _cacheManager.GetCacheTtl(assetType, isDetailData: false);
            await _cacheService.SetAsync(key, price, ttl, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing price in cache for {Symbol} ({AssetType})", price.Symbol, assetType);
        }
    }

    public async Task CacheBatchPricesAsync(IEnumerable<AssetPrice> prices, AssetType assetType, CancellationToken cancellationToken = default)
    {
        try
        {
            var ttl = _cacheManager.GetCacheTtl(assetType, isDetailData: false);
            var tasks = new List<Task>();
            
            foreach (var price in prices)
            {
                var key = _cacheManager.GeneratePriceKey(price.Symbol, assetType);
                tasks.Add(_cacheService.SetAsync(key, price, ttl, cancellationToken));
            }
            
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing batch prices in cache for asset type {AssetType}", assetType);
        }
    }

    public async Task<AssetDetail> GetDetailAsync(string symbol, AssetType assetType, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = _cacheManager.GenerateDetailKey(symbol, assetType);
            return await _cacheService.GetAsync<AssetDetail>(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving detail from cache for {Symbol} ({AssetType})", symbol, assetType);
            return null;
        }
    }

    public async Task CacheDetailAsync(AssetDetail detail, AssetType assetType, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = _cacheManager.GenerateDetailKey(detail.Symbol, assetType);
            var ttl = _cacheManager.GetCacheTtl(assetType, isDetailData: true);
            await _cacheService.SetAsync(key, detail, ttl, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing detail in cache for {Symbol} ({AssetType})", detail.Symbol, assetType);
        }
    }

}