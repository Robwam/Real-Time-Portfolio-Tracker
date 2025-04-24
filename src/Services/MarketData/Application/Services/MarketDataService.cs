using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketData.Application.Interfaces;
using MarketData.Models;
using Shared.Models.Enums;

namespace MarketData.Services;
public class MarketDataService : IMarketDataService
{
    private readonly IMarketDataCache _cache;
    private readonly IExternalClientFactory _clientFactory;
    private readonly ILogger<MarketDataService> _logger;

    public MarketDataService(
        IMarketDataCache cache,
        IExternalClientFactory clientFactory,
        ILogger<MarketDataService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AssetPrice> GetAssetPriceAsync(string symbol, AssetType assetType, bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));

        symbol = symbol.ToUpperInvariant().Trim();
        
        // Get from cache first (unless forcing refresh)
        AssetPrice price = null;
        if (!forceRefresh)
            price = await _cache.GetPriceAsync(symbol, assetType, cancellationToken);

        if (price != null)
            return price;
            
        // Get price from external API
        var client = _clientFactory.GetClient(assetType);
        price = await client.GetAssetPriceAsync(symbol, cancellationToken);
        
        // Cache the result
        if (price != null)
            await _cache.CachePriceAsync(price, assetType, cancellationToken);
            
        return price;
    }

    public async Task<IEnumerable<AssetPrice>> GetAssetPricesAsync(IEnumerable<string> symbols, AssetType? assetType = null, bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        if (symbols == null || !symbols.Any())
            throw new ArgumentException("Symbols cannot be null or empty", nameof(symbols));

        var normalizedSymbols = symbols.Select(s => s.ToUpperInvariant().Trim()).Distinct().ToList();
        
        // Process with specified asset type
        if (assetType.HasValue)
            return await ProcessSymbolsWithAssetType(normalizedSymbols, assetType.Value, forceRefresh, cancellationToken);
        
        // Group symbols by asset type and process in parallel
        var tasks = GroupSymbolsByAssetType(normalizedSymbols)
            .Select(group => ProcessSymbolsWithAssetType(group.Value, group.Key, forceRefresh, cancellationToken));
            
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r);
    }

    public async Task<AssetDetail> GetAssetDetailAsync(string symbol, AssetType assetType, bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));

        symbol = symbol.ToUpperInvariant().Trim();
        
        // Get from cache first (unless forcing refresh)
        AssetDetail detail = null;
        if (!forceRefresh)
            detail = await _cache.GetDetailAsync(symbol, assetType, cancellationToken);

        if (detail != null)
            return detail;
            
        // Get detail from external API
        var client = _clientFactory.GetClient(assetType);
        detail = await client.GetAssetDetailAsync(symbol, cancellationToken);
        
        // Cache the result
        if (detail != null)
            await _cache.CacheDetailAsync(detail, assetType, cancellationToken);
            
        return detail;
    }

    public async Task<bool> RefreshAssetCacheAsync(string symbol, AssetType assetType, CancellationToken cancellationToken = default)
    {
        try 
        {
            // Run price and detail refresh in parallel
            var priceTask = GetAssetPriceAsync(symbol, assetType, true, cancellationToken);
            var detailTask = GetAssetDetailAsync(symbol, assetType, true, cancellationToken);
            
            await Task.WhenAll(priceTask, detailTask);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh cache for {Symbol} ({AssetType})", symbol, assetType);
            return false;
        }
    }

    #region Helper Methods
    
    private async Task<IEnumerable<AssetPrice>> ProcessSymbolsWithAssetType(
        ICollection<string> symbols, 
        AssetType assetType, 
        bool forceRefresh,
        CancellationToken cancellationToken)
    {
        // First, check the cache for all symbols (unless forcing refresh)
        var (cachedPrices, missingSymbols) = !forceRefresh
            ? await GetCachedPrices(symbols, assetType, cancellationToken)
            : (new List<AssetPrice>(), symbols.ToList());
        
        // If all symbols were in cache, return early
        if (!missingSymbols.Any())
            return cachedPrices;
            
        // Get remaining symbols from external API
        var client = _clientFactory.GetClient(assetType);
        IEnumerable<AssetPrice> externalPrices;
        
        if (client.SupportsBatchRequests)
            externalPrices = await client.GetAssetPricesAsync(missingSymbols, cancellationToken);
        else
        {
            // Fetch individually in parallel
            var tasks = missingSymbols.Select(s => client.GetAssetPriceAsync(s, cancellationToken));
            externalPrices = (await Task.WhenAll(tasks)).Where(p => p != null);
        }
        
        // Cache the results (don't await to avoid delaying the response)
        if (externalPrices.Any())
            _ = _cache.CacheBatchPricesAsync(externalPrices, assetType, cancellationToken);
        
        // Combine cached and external results
        return cachedPrices.Concat(externalPrices);
    }
    
    private async Task<(List<AssetPrice> cachedPrices, List<string> missingSymbols)> GetCachedPrices(
        ICollection<string> symbols, 
        AssetType assetType, 
        CancellationToken cancellationToken)
    {
        var cachedPrices = new List<AssetPrice>();
        var missingSymbols = new List<string>();
        
        // Check cache for each symbol in parallel
        var tasks = symbols.Select(async symbol => {
            var price = await _cache.GetPriceAsync(symbol, assetType, cancellationToken);
            return (Symbol: symbol, Price: price);
        });
        
        var results = await Task.WhenAll(tasks);
        
        foreach (var result in results)
        {
            if (result.Price != null)
                cachedPrices.Add(result.Price);
            else
                missingSymbols.Add(result.Symbol);
        }
        
        return (cachedPrices, missingSymbols);
    }
    
    private Dictionary<AssetType, List<string>> GroupSymbolsByAssetType(ICollection<string> symbols)
    {
        var result = new Dictionary<AssetType, List<string>>();
        
        foreach (var symbol in symbols)
        {
            var assetType = InferAssetType(symbol);
            
            if (!result.ContainsKey(assetType))
                result[assetType] = new List<string>();
                
            result[assetType].Add(symbol);
        }
        
        return result;
    }
    
    private AssetType InferAssetType(string symbol)
    {
        // Simple logic to infer asset type from symbol pattern
        if (symbol.Length <= 5 && !symbol.Contains("-"))
            return AssetType.Stock;
            
        if (symbol.Contains("-USD") || symbol.Contains("BTC") || symbol.Contains("ETH"))
            return AssetType.Crypto;
            
        // Default to stock
        return AssetType.Stock;
    }

    public async Task<IDictionary<string, bool>> RefreshAssetsCacheAsync(
        IEnumerable<string> symbols,
        AssetType? assetType = null,
        CancellationToken cancellationToken = default)
    {
        if (symbols == null || !symbols.Any())
            throw new ArgumentException("Symbols cannot be null or empty", nameof(symbols));

        var normalizedSymbols = symbols.Select(s => s.ToUpperInvariant().Trim()).Distinct().ToList();
        var results = new Dictionary<string, bool>();
        
        // If specific asset type is provided
        if (assetType.HasValue)
        {
            // Refresh all symbols of the same type in parallel
            var refreshTasks = normalizedSymbols.ToDictionary(
                symbol => symbol,
                symbol => RefreshAssetCacheAsync(symbol, assetType.Value, cancellationToken));
            
            await Task.WhenAll(refreshTasks.Values);
            
            // Collect results
            foreach (var item in refreshTasks)
            {
                results[item.Key] = item.Value.Result;
            }
        }
        else
        {
            // Group symbols by inferred asset type
            var symbolsByAssetType = GroupSymbolsByAssetType(normalizedSymbols);
            
            // Process each group in parallel
            var groupTasks = new List<Task>();
            
            foreach (var group in symbolsByAssetType)
            {
                var assetTypeValue = group.Key;
                var groupSymbols = group.Value;
                
                var task = Task.Run(async () => {
                    var groupRefreshTasks = groupSymbols.ToDictionary(
                        symbol => symbol,
                        symbol => RefreshAssetCacheAsync(symbol, assetTypeValue, cancellationToken));
                    
                    await Task.WhenAll(groupRefreshTasks.Values);
                    
                    // Add results from this group to the main results dictionary
                    lock (results)
                    {
                        foreach (var item in groupRefreshTasks)
                        {
                            results[item.Key] = item.Value.Result;
                        }
                    }
                }, cancellationToken);
                
                groupTasks.Add(task);
            }
            
            // Wait for all groups to complete
            await Task.WhenAll(groupTasks);
        }
        
        return results;
    }

    #endregion
}