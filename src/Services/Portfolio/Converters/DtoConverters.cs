using Portfolio.Data.Entities;
using Shared.Models.DTOs;

namespace Portfolio.Converters;

public static class DtoConverter
{
    public static HoldingDto ToHoldingDto(Holding holding)
    {
        return new HoldingDto
        {
            Id = holding.Id,
            Symbol = holding.Symbol,
            AssetType = holding.AssetType,
            Quantity = holding.Quantity,
            AveragePurchasePrice = holding.AveragePurchasePrice,
            LastUpdated = holding.LastUpdated
        };
    }

    public static PortfolioDto ToPortfolioDto(Portfolio.Models.Portfolio portfolio)
    {
        return new PortfolioDto
        {
            UserId = portfolio.UserId,
            Holdings = portfolio.Holdings.Select(ToHoldingDto).ToList(),
            TotalValue = portfolio.TotalValue,
            LastUpdated = portfolio.LastUpdated
        };
    }
}
