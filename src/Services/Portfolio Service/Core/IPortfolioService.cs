public interface IPortfolioService
{
    // Get the entire portfolio for a user
    Task<UserPortfolio> GetPortfolioAsync(Guid userId);

    // Add a new holding to the user's portfolio
    Task<PortfolioHolding> AddHoldingAsync(Guid userId, CreateHoldingRequest request);

    // Update the metadata (info) of an existing holding in the portfolio (does not change price/quantity)
    Task<PortfolioHolding> UpdateHoldingInfoAsync(Guid userId, int holdingId, UpdateHoldingInfoRequest request);

    // Delete a holding from the portfolio
    Task<OperationResult> RemoveHoldingAsync(Guid userId, int holdingId);

    // Perform a buy or sell transaction for a holding in the portfolio (affects quantity/price)
    Task<Transaction> BuyOrSellTransactionAsync(Guid userId, CreateTransactionRequest request);

    // Get a summary of the user's portfolio (e.g., total value, number of holdings)
    Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(Guid userId);

    // Calculate the total value of the portfolio, converting all holdings to the base currency
    Task<decimal> CalculateTotalPortfolioValueAsync(Guid userId, string baseCurrency = "USD");
}
