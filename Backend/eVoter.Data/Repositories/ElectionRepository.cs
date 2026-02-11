using Microsoft.EntityFrameworkCore;
using eVoter.Core.Models;
using eVoter.Core.Interfaces;
using eVoter.Core.Enums;
using eVoter.Data.Context;

namespace eVoter.Data.Repositories;

public class ElectionRepository : Repository<Election>, IElectionRepository
{
    public ElectionRepository(eVoterDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Election>> GetActiveElectionsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(e => e.Candidates)
            .Where(e => e.Status == ElectionStatus.Active && e.StartDate <= now && e.EndDate >= now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Election>> GetElectionsByStatusAsync(ElectionStatus status)
    {
        return await _dbSet
            .Include(e => e.Candidates)
            .Where(e => e.Status == status)
            .ToListAsync();
    }

    public async Task<Election?> GetElectionWithCandidatesAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Candidates)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
