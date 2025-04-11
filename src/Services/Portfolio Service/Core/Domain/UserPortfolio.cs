using System;
using System.Collections.Generic;

namespace Core.Domain;

public class UserPortfolio
{
    public Guid UserId { get; set; }                             
    public List<PortfolioHolding> Holdings { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();

    // Optional metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}
