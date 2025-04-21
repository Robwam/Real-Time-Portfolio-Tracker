using System;
using Shared.Models.DTOs;
using Portfolio.Models;
using Portfolio.Converters;
using Portfolio.Data.Repository.Interfaces;
using Shared.Interfaces;
using PortfolioModel = Portfolio.Models.Portfolio;
using Portfolio.Services.Interfaces;
using Portfolio.Data.Entities;
using System.Collections;

namespace Portfolio.Services;

public class PortfolioService : IPortfolioService
{
    private readonly IPortfolioFactory _portfolioFactory;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IMarketDataClient _marketDataClient;

    public PortfolioService(
        IPortfolioFactory portfolioFactory,
        IPortfolioRepository portfolioRepository,
        IMarketDataClient marketDataClient)
    {
        _portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
        _portfolioRepository = portfolioRepository ?? throw new ArgumentNullException(nameof(portfolioRepository));
        _marketDataClient = marketDataClient ?? throw new ArgumentNullException(nameof(marketDataClient));
    }

    public async Task<HoldingDto> AddHoldingAsync(AddHoldingRequestDto request)
    {
        var price = await _marketDataClient.GetAssetPriceAsync(request.Symbol, request.AssetType);
        
        if (price == null)
            throw new ArgumentException($"Invalid symbol: {request.Symbol} or asset type: {request.AssetType}");

        // Check for existing holding with same symbol (for dollar-cost averaging)
        var existingHoldings = await _portfolioRepository.GetHoldingsAsync(request.UserId);
        var existingHolding = existingHoldings.FirstOrDefault(h => 
            h.Symbol.Equals(request.Symbol, StringComparison.OrdinalIgnoreCase) && 
            h.AssetType == request.AssetType);
        
        if (existingHolding != null)
        {
            existingHolding = await ApplyDollarCostAveraging(existingHolding, request.Quantity, request.PurchasePrice);
            var model = EntityToDomainConverter.ToHoldingModel(existingHolding, price);
            return DtoConverter.ToHoldingDto(model);
        }
        
        var newHolding = new Data.Entities.Holding
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Symbol = request.Symbol.ToUpperInvariant(),
            AssetType = request.AssetType,
            Quantity = request.Quantity,
            AveragePurchasePrice = request.PurchasePrice,
            LastUpdated = DateTime.UtcNow
        };

        var addedHolding = await _portfolioRepository.AddHoldingAsync(newHolding);
        if (addedHolding == null)
            throw new InvalidOperationException("Failed to add holding to database");
        
        var holdingModel = EntityToDomainConverter.ToHoldingModel(addedHolding, price);
        return DtoConverter.ToHoldingDto(holdingModel);
    }

    public async Task<bool> DeleteHoldingAsync(Guid holdingId)
    {
        return await _portfolioRepository.DeleteHoldingAsync(holdingId);
    }

    public async Task<HoldingDto?> GetHoldingAsync(Guid holdingId)
    {
        var entity = await _portfolioRepository.GetHoldingAsync(holdingId);
        if (entity == null) return null;
        
        var price = await _marketDataClient.GetAssetPriceAsync(entity.Symbol, entity.AssetType);
        var enrichedHolding = EntityToDomainConverter.ToHoldingModel(entity, price);
        
        return DtoConverter.ToHoldingDto(enrichedHolding);
    }

    public async Task<IEnumerable<HoldingDto>> GetHoldingsAsync(Guid userId)
    {
        var holdings = await _portfolioRepository.GetHoldingsAsync(userId);
        if (!holdings.Any()) return Enumerable.Empty<HoldingDto>();
        
        var symbols = holdings.Select(h => h.Symbol).Distinct().ToList();
        var prices = await _marketDataClient.GetAssetPricesAsync(symbols);
        var priceMap = prices.ToDictionary(p => p.Symbol, p => p);
        
        return holdings.Select(h => {
            priceMap.TryGetValue(h.Symbol, out var price);
            var model = EntityToDomainConverter.ToHoldingModel(h, price);
            return DtoConverter.ToHoldingDto(model);
        });
    }

    public async Task<PortfolioDto> GetPortfolioAsync(Guid userId)
    {
        var holdings = await _portfolioRepository.GetHoldingsAsync(userId);
        PortfolioModel portfolio;

        if (!holdings.Any())
        {
            portfolio = _portfolioFactory.CreateEmptyPortfolio(userId);
        }
        else
        {
            var symbols = holdings.Select(h => h.Symbol).Distinct().ToList();
            var marketData = await _marketDataClient.GetAssetPricesAsync(symbols);
            
            portfolio = _portfolioFactory.CreatePortfolio(userId, holdings, marketData);
        }
        
        return DtoConverter.ToPortfolioDto(portfolio);
    }

    public async Task<HoldingDto> UpdateHoldingAsync(UpdateHoldingRequestDto request)
    {
        var entity = await _portfolioRepository.GetHoldingAsync(request.HoldingId);
        if (entity == null) return null;
        
        entity.Quantity = request.Quantity;
        if (request.PurchasePrice.HasValue)
            entity.AveragePurchasePrice = request.PurchasePrice;
        entity.LastUpdated = DateTime.UtcNow;
        
        await _portfolioRepository.UpdateHoldingAsync(entity);
        
        var model = EntityToDomainConverter.ToHoldingModel(entity);
        return DtoConverter.ToHoldingDto(model);
    }

    private async Task<Data.Entities.Holding> ApplyDollarCostAveraging(
    Data.Entities.Holding existing, 
    decimal quantity, 
    decimal? purchasePrice)
{
    var totalQuantity = existing.Quantity + quantity;
    var totalCost = (existing.Quantity * (existing.AveragePurchasePrice ?? 0)) + 
                    (quantity * (purchasePrice ?? 0));
    
    existing.Quantity = totalQuantity;
    existing.AveragePurchasePrice = totalCost > 0 ? totalCost / totalQuantity : null;
    existing.LastUpdated = DateTime.UtcNow;
    
    await _portfolioRepository.UpdateHoldingAsync(existing);
    return existing;
}
}
