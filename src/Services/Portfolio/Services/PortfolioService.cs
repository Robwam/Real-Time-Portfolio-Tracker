using System;
using Shared.Models.DTOs;
using Portfolio.Models;
using Portfolio.Converters;
using Portfolio.Data.Repository.Interfaces;
using Portfolio.Services.Interfaces;
using PortfolioModel = Portfolio.Models.Portfolio;

namespace Portfolio.Services;

public class PortfolioService
{
    PortfolioFactory _portfolioFactory = new PortfolioFactory();
    IPortfolioRepository _portfolioRepository;
    IMarketDataClient _marketDataClient;


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
}
