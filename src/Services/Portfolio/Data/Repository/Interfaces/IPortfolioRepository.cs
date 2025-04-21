using Portfolio.Data.Entities;

namespace Portfolio.Data.Repository.Interfaces;
public interface IPortfolioRepository
{
    Task<IEnumerable<Holding>> GetHoldingsAsync(Guid userId);
    
    Task<Holding> GetHoldingAsync(Guid holdingId);
    
    Task<Holding> AddHoldingAsync(Holding holding);
    
    Task<bool> UpdateHoldingAsync(Holding holding);
    
    Task<bool> DeleteHoldingAsync(Guid holdingId);
}