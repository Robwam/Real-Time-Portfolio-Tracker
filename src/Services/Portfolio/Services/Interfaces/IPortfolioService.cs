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
        Task<HoldingDto> GetHoldingAsync(Guid holdingId);
        Task<HoldingDto> AddHoldingAsync(AddHoldingRequestDto request);
        Task<HoldingDto> UpdateHoldingAsync(UpdateHoldingRequestDto request);
        Task<bool> DeleteHoldingAsync(Guid holdingId);
    }
}