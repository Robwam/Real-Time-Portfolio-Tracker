namespace Shared.Models.DTOs;

#region Portfolio DTOs

// Response DTO for retrieving a complete portfolio
public class PortfolioDto
{
    public Guid UserId { get; set; }
    public ICollection<HoldingDto> Holdings { get; set; } = new List<HoldingDto>();
    public decimal TotalValue { get; set; }
    public decimal DailyChange { get; set; }
    public decimal DailyChangePercentage { get; set; }
    public DateTime LastUpdated { get; set; }
}

// Simplified portfolio summary (without holdings details)
public class PortfolioSummaryDto
{
    public Guid UserId { get; set; }
    public int HoldingsCount { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime LastUpdated { get; set; }
}

#endregion

#region Holding DTOs

// Response DTO for holdings
public class HoldingDto
{
    public Guid Id { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; } // e.g. "Apple Inc."
    public string AssetType { get; set; } // "Stock", "Crypto", etc.
    public decimal Quantity { get; set; }
    public decimal? AveragePurchasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal ProfitLossPercentage { get; set; }
    public DateTime LastUpdated { get; set; }
}

// Request DTO for adding a new holding
public class AddHoldingRequestDto
{
    public string Symbol { get; set; }
    public string AssetType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? PurchasePrice { get; set; }
}

// Request DTO for updating an existing holding
public class UpdateHoldingRequestDto
{
    public decimal Quantity { get; set; }
    public decimal? PurchasePrice { get; set; }
}

#endregion

#region Market Data DTOs

// Asset price information
public class AssetPriceDto
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public string AssetType { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercentage { get; set; }
    public DateTime LastUpdated { get; set; }
}

// Batch request for multiple prices
public class AssetPriceRequestDto
{
    public List<string> Symbols { get; set; }
    public string AssetType { get; set; }  // Optional filter
}

// Batch response for multiple prices
public class AssetPriceResponseDto
{
    public List<AssetPriceDto> Prices { get; set; }
    public DateTime AsOf { get; set; }
}

#endregion

#region Alert DTOs

// Price alert configuration
public class AlertDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Symbol { get; set; }
    public string AssetType { get; set; }
    public string AlertType { get; set; } // "PriceAbove", "PriceBelow", etc.
    public decimal TargetPrice { get; set; }
    public bool IsActive { get; set; }
    public string[] NotificationChannels { get; set; } // "Email", "InApp", etc.
}

// Request to create a new alert
public class CreateAlertRequestDto
{
    public string Symbol { get; set; }
    public string AssetType { get; set; }
    public string AlertType { get; set; }
    public decimal TargetPrice { get; set; }
    public string[] NotificationChannels { get; set; }
}

#endregion