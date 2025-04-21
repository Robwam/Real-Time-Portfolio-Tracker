using Shared.Interfaces;
using Shared.Models.DTOs;
using Shared.Models.Enums;

namespace Portfolio.Services;

public class MarketDataClient : IMarketDataClient
{
    public Task<AssetDetailDto> GetAssetDetailAsync(string symbol, AssetType assetType)
    {
        throw new NotImplementedException();
    }

    public Task<AssetPriceDto> GetAssetPriceAsync(string symbol, AssetType assetType)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AssetPriceDto>> GetAssetPricesAsync(IEnumerable<string> symbols, AssetType? assetType = null)
    {
        throw new NotImplementedException();
    }
}
