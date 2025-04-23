using MarketData.Application.Configuration;
using Shared.Models.Enums;
using Microsoft.Extensions.Options;

namespace MarketData.Infrastructure.Cache
{
    public class CacheKeyGenerator
    {
        private readonly MarketDataSettings _settings;

        public CacheKeyGenerator(IOptions<MarketDataSettings> settings)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public string GeneratePriceCacheKey(string symbol, AssetType assetType)
        {
            return $"{assetType}:{symbol}:price";
        }
        
        public string GenerateDetailCacheKey(string symbol, AssetType assetType)
        {
            return $"{assetType}:{symbol}:detail";
        }
        
        public TimeSpan GetCacheTtl(AssetType assetType, bool isDetailData = false)
        {
            if (isDetailData)
            {
                return assetType switch
                {
                    AssetType.Stock => TimeSpan.FromHours(_settings.CacheDurations.Stock.DetailHours),
                    AssetType.Crypto => TimeSpan.FromHours(_settings.CacheDurations.Crypto.DetailHours),
                    _ => TimeSpan.FromHours(_settings.CacheDurations.Default.DetailHours)
                };
            }
            else
            {
                return assetType switch
                {
                    AssetType.Stock => TimeSpan.FromMinutes(_settings.CacheDurations.Stock.PriceMinutes),
                    AssetType.Crypto => TimeSpan.FromMinutes(_settings.CacheDurations.Crypto.PriceMinutes),
                    _ => TimeSpan.FromMinutes(_settings.CacheDurations.Default.PriceMinutes)
                };
            }
        }
    }
}