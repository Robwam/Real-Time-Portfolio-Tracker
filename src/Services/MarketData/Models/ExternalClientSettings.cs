namespace MarketData.Models;

public class ExternalApiSettings
{
    public string ProviderName {get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public bool IsEnabled { get; set; } = true;
}

public class ExternalClientSettings
{
    public Dictionary<string, ExternalApiSettings> Providers { get; set; } = 
        new Dictionary<string, ExternalApiSettings>();
}