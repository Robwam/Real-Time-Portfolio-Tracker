using Portfolio.Data.Database;
using Portfolio.Data.Entities;
using Portfolio.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Data.Repository;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly PostgresContext _dbContext;

    public PortfolioRepository(PostgresContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Holding> AddHoldingAsync(Holding holding)
    {
        await _dbContext.Holdings.AddAsync(holding);
        await _dbContext.SaveChangesAsync();
        return holding;
    }

    public async Task<bool> DeleteHoldingAsync(Guid holdingId)
    {
        var holding = await _dbContext.Holdings.FindAsync(holdingId);
        if (holding == null)
            return false;

        _dbContext.Holdings.Remove(holding);
        var deletedCount = await _dbContext.SaveChangesAsync();
        return deletedCount > 0;
    }

    public async Task<Holding> GetHoldingAsync(Guid holdingId)
    {
        return await _dbContext.Holdings.FirstOrDefaultAsync(h => h.Id == holdingId);
    }

    public async Task<IEnumerable<Holding>> GetHoldingsAsync(Guid userId)
    {
        return await _dbContext.Holdings
            .Where(h => h.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> UpdateHoldingAsync(Holding holding)
    {
        _dbContext.Holdings.Update(holding);
        var updatedCount = await _dbContext.SaveChangesAsync();
        return updatedCount > 0;
    }
}