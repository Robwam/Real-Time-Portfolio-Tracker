using Shared.Models.DTOs;
using Shared.Models.Enums;

namespace MarketData.Application.Interfaces;

public interface IExternalMarketDataProvider
{
    Task<AssetPriceDto> FetchPriceAsync(
        string symbol, 
        AssetType assetType, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AssetPriceDto>> FetchBatchPricesAsync(
        IEnumerable<string> symbols, 
        AssetType assetType, 
        CancellationToken cancellationToken = default);

    Task<AssetDetailDto> FetchDetailAsync(
        string symbol, 
        AssetType assetType, 
        CancellationToken cancellationToken = default);

    bool SupportsBatchRequests(AssetType assetType);
}
