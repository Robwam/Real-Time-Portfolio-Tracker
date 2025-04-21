
using Shared.Models.DTOs;
namespace Portfolio.Services;

public static class EntityToDomainConverter
{
    public static Models.Holding ToHoldingModel(Data.Entities.Holding entity, AssetPriceDto? priceData = null)
    {
        var model = new Models.Holding
        {
            Id = entity.Id,
            Symbol = entity.Symbol,
            AssetType = entity.AssetType,
            Quantity = entity.Quantity,
            AveragePurchasePrice = entity.AveragePurchasePrice,
            LastUpdated = entity.LastUpdated
        };
        
        // Enrich with price data
        model.CurrentPrice = priceData != null ? priceData.CurrentPrice : 0;
        
        return model;
    }
}