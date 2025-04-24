using MarketData.Models;
using Shared.Models.Enums;

public class CacheKeyManager
{
    private readonly CacheSettings _settings;

    public CacheKeyManager(CacheSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public string GeneratePriceKey(string symbol, AssetType assetType)
    {
        return $"{assetType}:{symbol}:price";
    }

    public string GenerateDetailKey(string symbol, AssetType assetType)
    {
        return $"{assetType}:{symbol}:detail";
    }

    public TimeSpan GetCacheTtl(AssetType assetType, bool isDetailData)
    {
        if (isDetailData)
        {
            return assetType switch
            {
                AssetType.Stock => TimeSpan.FromHours(_settings.StockDetailHours),
                AssetType.Crypto => TimeSpan.FromHours(_settings.CryptoDetailHours),
                _ => TimeSpan.FromHours(24)
            };
        }
        else
        {
            return assetType switch
            {
                AssetType.Stock => TimeSpan.FromMinutes(_settings.StockPriceMinutes),
                AssetType.Crypto => TimeSpan.FromMinutes(_settings.CryptoPriceMinutes),
                _ => TimeSpan.FromMinutes(15)
            };
        }
    }
}

public class CacheSettings
{
    public int StockPriceMinutes { get; set; } = 15;
    public int CryptoPriceMinutes { get; set; } = 5;
    
    public int StockDetailHours { get; set; } = 24;
    public int CryptoDetailHours { get; set; } = 12;
}