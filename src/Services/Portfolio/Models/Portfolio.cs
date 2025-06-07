using System.Diagnostics.CodeAnalysis;

namespace Portfolio.Models;

public class Portfolio
{
    public Guid UserId { get; set; }
    public ICollection<Models.Holding> Holdings { get; set; } = new List<Models.Holding>();

    public decimal TotalValue { get; set; } = 0;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public void CalculateTotalValue(Dictionary<string, decimal> prices)
    {
        TotalValue = 0;

        foreach (var holding in Holdings)
        {
            if (prices.TryGetValue(holding.Symbol, out decimal price))
            {
                TotalValue += holding.Quantity * price;
            }
        }

        LastUpdated = DateTime.UtcNow;
    }
}

public class PortfolioEqualityComparer : IEqualityComparer<Portfolio>
{
    private readonly HoldingEqualityComparer _holdingComparer = new();

    public bool Equals(Portfolio? x, Portfolio? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        if (x.UserId != y.UserId)
            return false;

        if (Math.Abs(x.TotalValue - y.TotalValue) > 0.0001m)
            return false;

        if (Math.Abs((x.LastUpdated - y.LastUpdated).TotalSeconds) > 1)
            return false;

        if (x.Holdings.Count != y.Holdings.Count)
            return false;

        var xHoldingsSet = new HashSet<Holding>(x.Holdings, _holdingComparer);
        var yHoldingsSet = new HashSet<Holding>(y.Holdings, _holdingComparer);

        return xHoldingsSet.SetEquals(yHoldingsSet);
    }

    public int GetHashCode([DisallowNull] Portfolio obj)
    {
        throw new NotImplementedException();
    }
}

