namespace Core.DTO;

public class CreateHoldingRequest
{
    public string Symbol { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
}