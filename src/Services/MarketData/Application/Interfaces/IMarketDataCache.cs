using Shared.Models.DTOs;
using Shared.Models.Enums;

namespace MarketData.Application.Interfaces;
public interface IMarketDataCache
    {
        Task<AssetPriceDto> GetPriceAsync(
            string symbol, 
            AssetType assetType, 
            CancellationToken cancellationToken = default);

        Task CachePriceAsync(
            AssetPriceDto price, 
            AssetType assetType, 
            CancellationToken cancellationToken = default);

        Task CacheBatchPricesAsync(
            IEnumerable<AssetPriceDto> prices, 
            AssetType assetType, 
            CancellationToken cancellationToken = default);

        Task<AssetDetailDto> GetDetailAsync(
            string symbol, 
            AssetType assetType, 
            CancellationToken cancellationToken = default);

        Task CacheDetailAsync(
            AssetDetailDto detail, 
            string symbol, 
            AssetType assetType, 
            CancellationToken cancellationToken = default);
    }