using Shared.Models.Enums;

namespace Shared.Models.DTOs;

public class PortfolioDto
{
    public Guid UserId { get; set; }
    public ICollection<HoldingDto> Holdings { get; set; } = new List<HoldingDto>();
    public decimal TotalValue { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class HoldingDto
{
    public Guid Id { get; set; }
    public required string Symbol { get; set; }
    public AssetType AssetType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? CurrentPrice { get; set; }
    public decimal? AveragePurchasePrice { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class AssetPriceDto
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public AssetType AssetType { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercentage { get; set; }
    public string Currency { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class AssetDetailDto
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public AssetType AssetType { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercentage { get; set; }
    public decimal Volume { get; set; }
    public decimal MarketCap { get; set; }
    public decimal? DividendYield { get; set; }
    public decimal? PERatio { get; set; }
    public decimal? High52Week { get; set; }
    public decimal? Low52Week { get; set; }
    public string Description { get; set; }
    public string Exchange { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class UpdateHoldingRequestDto
{
    public Guid UserId { get; init; }
    public Guid HoldingId { get; init; }
    public decimal Quantity { get; init; }
    public decimal? PurchasePrice { get; init; }
}

public class AddHoldingRequestDto
{
    public Guid UserId { get; init; }
    public string Symbol { get; init; }
    public decimal Quantity { get; init; }
    public AssetType AssetType { get; init; }
    public decimal? PurchasePrice { get; init; }
}