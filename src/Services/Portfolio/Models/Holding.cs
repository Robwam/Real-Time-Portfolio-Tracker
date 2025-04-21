using Shared.Models.Enums;

namespace Portfolio.Models;

public class Holding
{
    public Guid Id { get; set; }
    public string Symbol { get; set; }
    public AssetType AssetType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? AveragePurchasePrice { get; set; }
    
    // Non-persisted, calculated properties
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