using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.Models.DTOs;
using Shared.Models.Enums;

namespace MarketData.Application.Interfaces
{
    /// <summary>
    /// Core service for retrieving market data from both cache and external sources.
    /// This internal service is used by the MarketDataClient that implements the public interface.
    /// </summary>
    public interface IMarketDataService
    {
        /// <summary>
        /// Gets the current price for a specific asset.
        /// </summary>
        /// <param name="symbol">The asset symbol.</param>
        /// <param name="assetType">The type of asset.</param>
        /// <param name="forceRefresh">If true, bypasses the cache and fetches fresh data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asset price information.</returns>
        Task<AssetPriceDto> GetAssetPriceAsync(
            string symbol, 
            AssetType assetType, 
            bool forceRefresh = false, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current prices for multiple assets.
        /// </summary>
        /// <param name="symbols">List of asset symbols.</param>
        /// <param name="assetType">Optional type of assets. If provided, all symbols are assumed to be of this type.</param>
        /// <param name="forceRefresh">If true, bypasses the cache and fetches fresh data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of asset price information.</returns>
        Task<IEnumerable<AssetPriceDto>> GetAssetPricesAsync(
            IEnumerable<string> symbols, 
            AssetType? assetType = null, 
            bool forceRefresh = false, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets detailed information for a specific asset.
        /// </summary>
        /// <param name="symbol">The asset symbol.</param>
        /// <param name="assetType">The type of asset.</param>
        /// <param name="forceRefresh">If true, bypasses the cache and fetches fresh data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Detailed asset information.</returns>
        Task<AssetDetailDto> GetAssetDetailAsync(
            string symbol, 
            AssetType assetType, 
            bool forceRefresh = false, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes the cached data for a specific asset.
        /// </summary>
        /// <param name="symbol">The asset symbol.</param>
        /// <param name="assetType">The type of asset.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if refresh was successful.</returns>
        Task<bool> RefreshAssetCacheAsync(
            string symbol, 
            AssetType assetType, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes the cached data for multiple assets.
        /// </summary>
        /// <param name="symbols">List of asset symbols.</param>
        /// <param name="assetType">Optional type of assets. If provided, all symbols are assumed to be of this type.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Dictionary with results of refresh operations per symbol.</returns>
        Task<IDictionary<string, bool>> RefreshAssetsCacheAsync(
            IEnumerable<string> symbols, 
            AssetType? assetType = null, 
            CancellationToken cancellationToken = default);
    }
}