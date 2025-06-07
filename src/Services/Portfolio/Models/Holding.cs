using System.Diagnostics.CodeAnalysis;
using Shared.Models.Enums;

namespace Portfolio.Models;

public class Holding
{
    public Guid Id { get; set; }
    public string Symbol { get; set; }
    public AssetType AssetType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? AveragePurchasePrice { get; set; }

    public decimal CurrentPrice { get; set; }
    public decimal CurrentValue => Quantity * CurrentPrice;
    public decimal ProfitLoss =>
        AveragePurchasePrice.HasValue
            ? Quantity * (CurrentPrice - AveragePurchasePrice.Value)
            : 0;
    public decimal ProfitLossPercentage =>
        AveragePurchasePrice.HasValue && AveragePurchasePrice.Value > 0
            ? ((CurrentPrice - AveragePurchasePrice.Value) / AveragePurchasePrice.Value) * 100
            : 0;
    public DateTime LastUpdated { get; set; }
}

public class HoldingEqualityComparer : IEqualityComparer<Holding>
{
    public bool Equals(Holding? x, Holding? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return x.Id == y.Id &&
               string.Equals(x.Symbol, y.Symbol, StringComparison.OrdinalIgnoreCase) &&
               x.AssetType == y.AssetType &&
               x.Quantity == y.Quantity &&
               Nullable.Equals(x.AveragePurchasePrice, y.AveragePurchasePrice) &&
               x.LastUpdated == y.LastUpdated;
    }

    public int GetHashCode([DisallowNull] Holding obj)
    {
        throw new NotImplementedException();
    }
}
