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
