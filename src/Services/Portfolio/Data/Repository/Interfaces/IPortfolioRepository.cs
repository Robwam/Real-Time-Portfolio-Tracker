using Portfolio.Data.Entities;

namespace Portfolio.Data.Repository.Interfaces
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