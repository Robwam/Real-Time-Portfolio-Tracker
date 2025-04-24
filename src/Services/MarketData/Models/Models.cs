using System;
using Shared.Models.Enums;

namespace MarketData.Models;

public class AssetPrice
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? PreviousClose { get; set; }
    public long? Volume { get; set; }
    public DateTime LastUpdated { get; set; }
    
    // Calculated properties
    public decimal? Change => PreviousClose.HasValue ? Price - PreviousClose.Value : null;
    public decimal? ChangePercent => PreviousClose.HasValue && PreviousClose.Value != 0 
        ? (Price - PreviousClose.Value) / PreviousClose.Value * 100 
        : null;
}

public class AssetDetail
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public AssetType AssetType { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public decimal? MarketCap { get; set; }
    public decimal? PeRatio { get; set; }
    public decimal? DividendYield { get; set; }
    public decimal? YearHigh { get; set; }
    public decimal? YearLow { get; set; }
    public DateTime LastUpdated { get; set; }
}