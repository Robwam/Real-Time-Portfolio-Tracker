using Shared.Models.DTOs;
using Shared.Models.Enums;
using MarketData.Models;

namespace MarketData.Application.Interfaces;
public interface IMarketDataCache
{
    Task<AssetPrice> GetPriceAsync(
        string symbol, 
        AssetType assetType, 
        CancellationToken cancellationToken = default);

    Task CachePriceAsync(
        AssetPrice price, 
        AssetType assetType, 
        CancellationToken cancellationToken = default);

    Task CacheBatchPricesAsync(
        IEnumerable<AssetPrice> prices, 
        AssetType assetType, 
        CancellationToken cancellationToken = default);

    Task<AssetDetail> GetDetailAsync(
        string symbol,
        AssetType assetType, 
        CancellationToken cancellationToken = default);

    Task CacheDetailAsync(
        AssetDetail detail, 
        AssetType assetType, 
        CancellationToken cancellationToken = default);
}