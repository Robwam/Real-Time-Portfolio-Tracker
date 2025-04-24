    using MarketData.Models;

    namespace MarketData.Application.Interfaces;

    public interface IExternalClient
    {
        bool SupportsBatchRequests { get; }
        Task<AssetPrice> GetAssetPriceAsync(string symbol, CancellationToken cancellationToken = default);
        Task<IEnumerable<AssetPrice>> GetAssetPricesAsync(IEnumerable<string> symbols, CancellationToken cancellationToken = default);
        Task<AssetDetail> GetAssetDetailAsync(string symbol, CancellationToken cancellationToken = default);
    }