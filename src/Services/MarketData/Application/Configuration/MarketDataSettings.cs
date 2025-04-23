namespace MarketData.Application.Configuration;
public class MarketDataSettings
{
    public CacheDurationsConfig CacheDurations { get; set; } = new();
    public ProvidersConfig Providers { get; set; } = new();
    public Dictionary<string, string> ApiKeys { get; set; } = new();

    public class CacheDurationsConfig
    {
        public AssetCacheConfig Stock { get; set; } = new();
        public AssetCacheConfig Crypto { get; set; } = new();
        public AssetCacheConfig Default { get; set; } = new();
    }

    public class AssetCacheConfig
    {
        public int PriceMinutes { get; set; }
        public int DetailHours { get; set; }
    }

    public class ProvidersConfig
    {
        public ProviderConfig Stock { get; set; } = new();
        public ProviderConfig Crypto { get; set; } = new();
    }

    public class ProviderConfig
    {
        public string Primary { get; set; }
        public string Fallback { get; set; }
    }
}