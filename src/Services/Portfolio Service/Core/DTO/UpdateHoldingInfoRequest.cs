using System;

namespace Core.DTO;

public class UpdateHoldingInfoRequest
{
    public string? Name { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? Currency { get; set; }
}
