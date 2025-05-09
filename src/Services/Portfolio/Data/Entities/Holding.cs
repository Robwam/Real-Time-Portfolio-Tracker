using System.ComponentModel.DataAnnotations;
using Shared.Models.Enums;

namespace Portfolio.Data.Entities;

public class Holding
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Symbol { get; set; }
    public AssetType AssetType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? AveragePurchasePrice { get; set; }
    public DateTime LastUpdated { get; set; }
}