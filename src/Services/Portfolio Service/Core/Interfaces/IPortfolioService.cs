using Core.DTO;
using Core.Domain;

namespace Core.Interfaces;

public interface IPortfolioService
{
    Task<UserPortfolio> GetPortfolioAsync(Guid userId);

    Task<PortfolioHolding> AddHoldingAsync(Guid userId, CreateHoldingRequest request);

    // Update the metadata (info) of an existing holding in the portfolio (does not change price/quantity)
    Task<PortfolioHolding> UpdateHoldingInfoAsync(Guid userId, int holdingId, UpdateHoldingInfoRequest request);

    Task<bool> RemoveHoldingAsync(Guid userId, int holdingId);

    Task<Transaction> BuyOrSellTransactionAsync(Guid userId, CreateTransactionRequest request);

    Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(Guid userId);

    Task<decimal> CalculateTotalPortfolioValueAsync(Guid userId);
}
