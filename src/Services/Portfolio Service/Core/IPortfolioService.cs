public interface IPortfolioService
{
    // Get the entire portfolio for a user
    Task<UserPortfolio> GetPortfolioAsync(string userId);

    // Add a new holding to the user's portfolio
    Task<PortfolioHolding> AddHoldingAsync(string userId, CreateHoldingRequest request);

    // Update an existing holding in the portfolio
    Task<PortfolioHolding> UpdateHoldingAsync(string userId, int holdingId, UpdateHoldingRequest request);

    // Delete a holding from the portfolio
    Task<bool> RemoveHoldingAsync(string userId, int holdingId);

    // Add a transaction (buy/sell) to the portfolio
    Task<Transaction> AddTransactionAsync(string userId, CreateTransactionRequest request);

    // Get a summary of the user's portfolio (e.g., total value, number of holdings)
    Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(string userId);

    // Calculate the current value of the portfolio
    Task<decimal> CalculateTotalPortfolioValueAsync(string userId);

    // Get the current price of a specific holding (could be a stock, for example)
    Task<decimal> GetHoldingCurrentPriceAsync(string symbol);

    // Validate a transaction (check if the user has enough holdings to sell, etc.)
    Task<ValidationResult> ValidateTransactionAsync(string userId, Transaction transaction);
}
