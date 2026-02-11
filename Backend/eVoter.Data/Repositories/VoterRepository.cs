using Microsoft.EntityFrameworkCore;
using eVoter.Core.Models;
using eVoter.Core.Interfaces;
using eVoter.Data.Context;

namespace eVoter.Data.Repositories;

public class VoterRepository : Repository<Voter>, IVoterRepository
{
    public VoterRepository(eVoterDbContext context) : base(context)
    {
    }

    public async Task<Voter?> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(v => v.User)
            .Include(v => v.BiometricData)
            .FirstOrDefaultAsync(v => v.UserId == userId);
    }

    public async Task<Voter?> GetByNationalIdAsync(string nationalId)
    {
        return await _dbSet
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.NationalId == nationalId);
    }

    public async Task<bool> HasVotedInElectionAsync(int voterId, int electionId)
    {
        return await _context.Votes
            .AnyAsync(v => v.VoterId == voterId && v.ElectionId == electionId);
    }
}
