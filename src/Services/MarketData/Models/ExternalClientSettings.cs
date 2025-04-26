namespace MarketData.Models;

public class ExternalClientSettings
{
    public string StockProvider { get; set; } = "AlphaVantage";
    public string CryptoProvider { get; set; } = "CoinGecko";
    
    public string AlphaVantageApiKey { get; set; }
    public string CoinGeckoApiKey { get; set; }
}