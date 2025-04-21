using Shared.Interfaces;
using Shared.Models.DTOs;
using Shared.Models.Enums;

namespace Portfolio.Services;

public class MarketDataClient : IMarketDataClient
{
    public async Task<AssetDetailDto> GetAssetDetailAsync(string symbol, AssetType assetType)
    {
        return new AssetDetailDto{Symbol = "AAPL", Name = "Apple"};
    }

    public async Task<AssetPriceDto> GetAssetPriceAsync(string symbol, AssetType assetType)
    {
        return new AssetPriceDto{Name = "AAPL", AssetType = AssetType.Stock, CurrentPrice = 150.00m};
    }

    public async Task<IEnumerable<AssetPriceDto>> GetAssetPricesAsync(IEnumerable<string> symbols, AssetType? assetType = null)
    {
        var assets = new List<AssetPriceDto>();
        assets.Add(new AssetPriceDto{Name = "AAPL", AssetType = AssetType.Stock, CurrentPrice = 150.00m});
        assets.Add(new AssetPriceDto{Name = "GOOGL", AssetType = AssetType.Stock, CurrentPrice = 2800.00m});

        return assets;
    }
}
