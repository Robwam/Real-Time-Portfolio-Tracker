using Shared.Models.Enums;
using Portfolio.Models;
using Portfolio.Data.Entities;
using Holding = Portfolio.Models.Holding;

namespace Portfolio.Tests;

public class HelperMethods
{
    private readonly DateTime _fixedDateTime = new DateTime(2025, 5, 12, 10, 30, 0, DateTimeKind.Utc);
    
    public Portfolio.Models.Portfolio CreateTestPortfolio()
    {
        return new Portfolio.Models.Portfolio 
        {
            UserId = Guid.Empty,
            Holdings = new List<Holding>
            {
                new Holding
                {
                    Id = Guid.NewGuid(),
                    Symbol = "AAPL",
                    AssetType = AssetType.Stock,
                    Quantity = 10,
                    AveragePurchasePrice = 150.00m,
                    CurrentPrice = 155.00m,
                    LastUpdated = _fixedDateTime
                },
                new Holding
                {
                    Id = Guid.NewGuid(),
                    Symbol = "BTC",
                    AssetType = AssetType.Crypto,
                    Quantity = 0.5m,
                    AveragePurchasePrice = 30000.00m,
                    CurrentPrice = 31000.00m,
                    LastUpdated = _fixedDateTime
                }
            },
            TotalValue = 17050m,
            LastUpdated = _fixedDateTime
        };
    }
}
