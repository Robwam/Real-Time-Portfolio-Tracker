namespace Portfolio.Models;

public class Portfolio
{
    public Guid UserId { get; set; }
    public ICollection<Data.Entities.Holding> Holdings { get; set; } = new List<Data.Entities.Holding>();
    
    public decimal TotalValue { get; private set; }
    public DateTime LastUpdated { get; private set; }
    
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
