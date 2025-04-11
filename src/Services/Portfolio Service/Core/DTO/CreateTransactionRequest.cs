using System;
using Core.Domain.Enums;

namespace Core.DTO;

public class CreateTransactionRequest
{
    public int HoldingId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public TransactionType Type { get; set; }
}
