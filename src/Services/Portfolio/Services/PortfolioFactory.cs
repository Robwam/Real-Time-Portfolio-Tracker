using Shared.Models.DTOs;
using Portfolio.Data.Entities;
using Portfolio.Models;

namespace Portfolio.Services;

public interface IPortfolioFactory
{
    Models.Portfolio CreateEmptyPortfolio(Guid userId);
    Models.Portfolio CreatePortfolio(
        Guid userId,
        IEnumerable<Data.Entities.Holding> entityHoldings,
        IEnumerable<AssetPriceDto> marketData);
}

public class PortfolioFactory : IPortfolioFactory
{
    public Models.Portfolio CreateEmptyPortfolio(Guid userId)
    {
        return new Models.Portfolio
        {
            UserId = userId,
            Holdings = new List<Models.Holding>(),
            TotalValue = 0,
            LastUpdated = DateTime.UtcNow
        };
    }

    public Models.Portfolio CreatePortfolio(
        Guid userId,
        IEnumerable<Data.Entities.Holding> entityHoldings,
        IEnumerable<AssetPriceDto> marketData)
    {
        var priceMap = marketData.ToDictionary(p => p.Symbol, p => p);
        var portfolio = new Models.Portfolio
        {
            UserId = userId,
            Holdings = new List<Models.Holding>(),
            LastUpdated = DateTime.UtcNow
        };

        foreach (var entity in entityHoldings)
        {
            priceMap.TryGetValue(entity.Symbol, out var priceData);
            
            var domainHolding = new Models.Holding
            {
                Id = entity.Id,
                Symbol = entity.Symbol,
                AssetType = entity.AssetType,
                Quantity = entity.Quantity,
                AveragePurchasePrice = entity.AveragePurchasePrice,
                CurrentPrice = priceData?.CurrentPrice ?? 0,
                LastUpdated = priceData?.LastUpdated ?? entity.LastUpdated
            };
            
            portfolio.Holdings.Add(domainHolding);
        }

        portfolio.CalculateTotalValue(priceMap.ToDictionary(p => p.Key, p => p.Value.CurrentPrice));
        
        return portfolio;
    }
}