using Shared.Models.Enums;
using Shared.Models.DTOs;

namespace Shared.Interfaces;

public interface IMarketDataClient
{
    Task<AssetPriceDto> GetAssetPriceAsync(string symbol, AssetType assetType);
    
    Task<IEnumerable<AssetPriceDto>> GetAssetPricesAsync(IEnumerable<string> symbols, AssetType? assetType = null);
    
    Task<AssetDetailDto> GetAssetDetailAsync(string symbol, AssetType assetType);
}
