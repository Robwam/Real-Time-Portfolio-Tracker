// Location: Portfolio/Data/Repositories/Interfaces/IPortfolioRepository.cs

using Portfolio.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portfolio.Data.Repositories.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<Holding>> GetHoldingsAsync(Guid userId);
        
        Task<Holding> GetHoldingAsync(Guid userId, Guid holdingId);
        
        Task<Holding> AddHoldingAsync(Guid userId, Holding holding);
        
        Task<bool> UpdateHoldingAsync(Guid userId, Holding holding);
        
        Task<bool> DeleteHoldingAsync(Guid userId, Guid holdingId);
    }
}