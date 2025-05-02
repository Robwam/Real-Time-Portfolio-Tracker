using Shared.Interfaces;
using Shared.Models.DTOs;
using Shared.Models.Enums;
using MarketData.Services;

namespace Portfolio.Services;

public class MarketDataClient : IMarketDataClient
{
    private readonly HttpClient _httpClient;

    public MarketDataClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AssetDetailDto> GetAssetDetailAsync(string symbol, AssetType assetType)
    {
        throw new NotImplementedException("Not yet implemented");
    }

    public async Task<AssetPriceDto> GetAssetPriceAsync(string symbol, AssetType assetType)
    {
        var response = await _httpClient.GetAsync($"api/price/{symbol}?assetType={assetType}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AssetPriceDto>();
    }

    public async Task<IEnumerable<AssetPriceDto>> GetAssetPricesAsync(IEnumerable<string> symbols, AssetType? assetType = null)
    {
        throw new NotImplementedException("Not yet implemented");
    }
}
