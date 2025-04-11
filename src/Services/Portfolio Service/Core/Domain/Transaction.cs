using Core.Domain.Enums;

namespace Core.Domain;

public class Transaction
{
    public int Id { get; set; }                  // Database ID or unique ID
    public Guid UserId { get; set; }             // Who made the transaction
    public int HoldingId { get; set; }           // Which holding this affects
    public TransactionType Type { get; set; }    // Buy or Sell
    public decimal Quantity { get; set; }        // How much bought/sold
    public decimal PricePerUnit { get; set; }    // Price at the time of trade
    public DateTime Timestamp { get; set; }      // When it was executed
    public string? Notes { get; set; }           // Optional comment
}
