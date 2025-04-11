using System;

namespace Core.DTO;

public class PortfolioSummaryResponse
{
    public decimal TotalValue { get; set; }
    public int CountHoldings { get; set; }
    public List<PortfolioHoldingResponse> Holdings { get; set; } = new();
}

public class PortfolioHoldingResponse
{
    public string Symbol { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal CurrentValue { get; set; }
}
