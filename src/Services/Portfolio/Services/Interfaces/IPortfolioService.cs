using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portfolio.Services.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioDto> GetPortfolioAsync(Guid userId);
        Task<IEnumerable<HoldingDto>> GetHoldingsAsync(Guid userId);
        Task<HoldingDto> GetHoldingAsync(Guid userId, Guid holdingId);
        Task<HoldingDto> AddHoldingAsync(Guid userId, AddHoldingRequestDto request);
        Task<HoldingDto> UpdateHoldingAsync(Guid userId, Guid holdingId, UpdateHoldingRequestDto request);
        Task<bool> DeleteHoldingAsync(Guid userId, Guid holdingId);
    }
}