using Portfolio.Models;
using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortfolioModel = Portfolio.Models.Portfolio;

namespace Portfolio.Services.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioModel> GetPortfolioAsync(Guid userId);
        Task<IEnumerable<Holding>> GetHoldingsAsync(Guid userId);
        Task<Holding> GetHoldingAsync(Guid holdingId);
        Task<Holding> AddHoldingAsync(AddHoldingRequestDto request);
        Task<Holding> UpdateHoldingAsync(UpdateHoldingRequestDto request);
        Task<bool> DeleteHoldingAsync(Guid holdingId);
    }
}