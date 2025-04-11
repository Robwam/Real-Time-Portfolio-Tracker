using System;

namespace Core.Domain;

public class PortfolioHolding
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Symbol { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal AveragePrice { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
}
