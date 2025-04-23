using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Models.DTOs;
using Shared.Models.Enums;
using MarketData.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketData.Application.Services
{
    public class MarketDataOrchestrator : IMarketDataService
    {
        private readonly IMarketDataCache _cache;
        private readonly IExternalMarketDataProvider _externalProvider;
        private readonly SymbolValidator _symbolValidator;
        private readonly ILogger<MarketDataOrchestrator> _logger;

        public MarketDataOrchestrator(
            IMarketDataCache cache,
            IExternalMarketDataProvider externalProvider,
            SymbolValidator symbolValidator,
            ILogger<MarketDataOrchestrator> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _externalProvider = externalProvider ?? throw new ArgumentNullException(nameof(externalProvider));
            _symbolValidator = symbolValidator ?? throw new ArgumentNullException(nameof(symbolValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AssetPriceDto> GetAssetPriceAsync(
            string symbol, 
            AssetType assetType, 
            bool forceRefresh = false, 
            CancellationToken cancellationToken = default)
        {
            var normalizedSymbol = _symbolValidator.NormalizeAndValidateSymbol(symbol);
            
            // Try cache first if not forcing refresh
            AssetPriceDto price = null;
            if (!forceRefresh)
            {
                price = await _cache.GetPriceAsync(normalizedSymbol, assetType, cancellationToken);
                if (price != null)
                {
                    _logger.LogDebug($"Cache hit for asset price {normalizedSymbol} ({assetType})");
                    return price;
                }
                
                _logger.LogDebug($"Cache miss for asset price {normalizedSymbol} ({assetType})");
            }

            // Get from external provider and cache
            try
            {
                price = await _externalProvider.FetchPriceAsync(normalizedSymbol, assetType, cancellationToken);
                
                if (price != null)
                {
                    await _cache.CachePriceAsync(price, assetType, cancellationToken);
                    _logger.LogInformation($"Updated price cache for {normalizedSymbol} ({assetType})");
                }
                
                return price;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching price for {normalizedSymbol} ({assetType})");
                throw;
            }
        }

        public async Task<IEnumerable<AssetPriceDto>> GetAssetPricesAsync(
            IEnumerable<string> symbols, 
            AssetType? assetType = null, 
            bool forceRefresh = false, 
            CancellationToken cancellationToken = default)
        {
            if (symbols == null || !symbols.Any())
            {
                throw new ArgumentException("Symbols cannot be null or empty", nameof(symbols));
            }

            var normalizedSymbols = _symbolValidator.NormalizeAndValidateSymbols(symbols);
            
            // If specific asset type is provided, use batch processing
            if (assetType.HasValue)
            {
                return await GetAssetPricesWithTypeAsync(normalizedSymbols, assetType.Value, forceRefresh, cancellationToken);
            }
            
            // If no asset type is provided, process each symbol individually
            return await GetAssetPricesWithoutTypeAsync(normalizedSymbols, forceRefresh, cancellationToken);
        }

        public async Task<AssetDetailDto> GetAssetDetailAsync(
            string symbol, 
            AssetType assetType, 
            bool forceRefresh = false, 
            CancellationToken cancellationToken = default)
        {
            _symbolValidator.ValidateSymbol(symbol);
            string normalizedSymbol = _symbolValidator.NormalizeSymbol(symbol);
            
            // Try cache first if not forcing refresh
            AssetDetailDto detail = null;
            if (!forceRefresh)
            {
                detail = await _cache.GetDetailAsync(normalizedSymbol, assetType, cancellationToken);
                if (detail != null)
                {
                    _logger.LogDebug("Cache hit for asset detail {Symbol} ({AssetType})", normalizedSymbol, assetType);
                    return detail;
                }
                
                _logger.LogDebug("Cache miss for asset detail {Symbol} ({AssetType})", normalizedSymbol, assetType);
            }

            // Get from external provider and cache
            try
            {
                detail = await _externalProvider.FetchDetailAsync(normalizedSymbol, assetType, cancellationToken);
                
                if (detail != null)
                {
                    await _cache.CacheDetailAsync(detail, normalizedSymbol, assetType, cancellationToken);
                    _logger.LogInformation("Updated detail cache for {Symbol} ({AssetType})", normalizedSymbol, assetType);
                }
                
                return detail;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching detail for {Symbol} ({AssetType})", normalizedSymbol, assetType);
                throw;
            }
        }

        public async Task<bool> RefreshAssetCacheAsync(
            string symbol, 
            AssetType assetType, 
            CancellationToken cancellationToken = default)
        {
            try 
            {
                // Force refresh both price and detail
                await GetAssetPriceAsync(symbol, assetType, true, cancellationToken);
                await GetAssetDetailAsync(symbol, assetType, true, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh cache for {Symbol} ({AssetType})", symbol, assetType);
                return false;
            }
        }

        public async Task<IDictionary<string, bool>> RefreshAssetsCacheAsync(
            IEnumerable<string> symbols, 
            AssetType? assetType = null, 
            CancellationToken cancellationToken = default)
        {
            if (symbols == null || !symbols.Any())
            {
                throw new ArgumentException("Symbols cannot be null or empty", nameof(symbols));
            }

            var normalizedSymbols = _symbolValidator.NormalizeAndValidateSymbols(symbols);
            var results = new Dictionary<string, bool>();
            
            if (assetType.HasValue)
            {
                // Refresh prices first using batch capabilities
                await GetAssetPricesAsync(normalizedSymbols, assetType, true, cancellationToken);
                
                // Refresh details individually
                foreach (var symbol in normalizedSymbols)
                {
                    try
                    {
                        await GetAssetDetailAsync(symbol, assetType.Value, true, cancellationToken);
                        results[symbol] = true;
                    }
                    catch
                    {
                        results[symbol] = false;
                    }
                }
            }
            else
            {
                // Process each symbol individually with inferred asset type
                foreach (var symbol in normalizedSymbols)
                {
                    var inferredAssetType = _symbolValidator.InferAssetType(symbol);
                    results[symbol] = await RefreshAssetCacheAsync(symbol, inferredAssetType, cancellationToken);
                }
            }
            
            return results;
        }

        #region Private Helper Methods

        private async Task<IEnumerable<AssetPriceDto>> GetAssetPricesWithTypeAsync(
            IReadOnlyCollection<string> symbols, 
            AssetType assetType, 
            bool forceRefresh, 
            CancellationToken cancellationToken)
        {
            var results = new List<AssetPriceDto>();
            var missingSymbols = new List<string>();
            
            // Check cache first if not forcing refresh
            if (!forceRefresh)
            {
                foreach (var symbol in symbols)
                {
                    var cachedPrice = await _cache.GetPriceAsync(symbol, assetType, cancellationToken);
                    
                    if (cachedPrice != null)
                    {
                        results.Add(cachedPrice);
                    }
                    else
                    {
                        missingSymbols.Add(symbol);
                    }
                }
            }
            else
            {
                missingSymbols = symbols.ToList();
            }
            
            // If we have symbols not found in cache, fetch them
            if (missingSymbols.Any())
            {
                try
                {
                    if (_externalProvider.SupportsBatchRequests(assetType))
                    {
                        // Use batch request
                        var fetchedPrices = await _externalProvider.FetchBatchPricesAsync(missingSymbols, assetType, cancellationToken);
                        
                        if (fetchedPrices != null && fetchedPrices.Any())
                        {
                            results.AddRange(fetchedPrices);
                            await _cache.CacheBatchPricesAsync(fetchedPrices, assetType, cancellationToken);
                        }
                    }
                    else
                    {
                        // Fetch individually
                        var fetchTasks = missingSymbols.Select(symbol => GetAssetPriceAsync(symbol, assetType, true, cancellationToken));
                        var fetchedPrices = await Task.WhenAll(fetchTasks);
                        results.AddRange(fetchedPrices.Where(p => p != null));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching prices for multiple symbols of type {AssetType}", assetType);
                    throw;
                }
            }
            
            return results;
        }

        private async Task<IEnumerable<AssetPriceDto>> GetAssetPricesWithoutTypeAsync(
            IReadOnlyCollection<string> symbols, 
            bool forceRefresh, 
            CancellationToken cancellationToken)
        {
            // For mixed asset types, process each symbol individually
            var tasks = symbols.Select(symbol =>
            {
                var inferredAssetType = _symbolValidator.InferAssetType(symbol);
                return GetAssetPriceAsync(symbol, inferredAssetType, forceRefresh, cancellationToken);
            }).ToList();
            
            var prices = await Task.WhenAll(tasks);
            return prices.Where(p => p != null);
        }

        #endregion
    }
}