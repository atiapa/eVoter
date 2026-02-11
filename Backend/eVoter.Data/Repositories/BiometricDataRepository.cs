using Microsoft.EntityFrameworkCore;
using eVoter.Core.Models;
using eVoter.Core.Interfaces;
using eVoter.Data.Context;

namespace eVoter.Data.Repositories;

public class BiometricDataRepository : Repository<BiometricData>, IBiometricDataRepository
{
    public BiometricDataRepository(eVoterDbContext context) : base(context)
    {
    }

    public async Task<BiometricData?> GetByVoterIdAsync(int voterId)
    {
        return await _dbSet
            .Include(b => b.Voter)
            .FirstOrDefaultAsync(b => b.VoterId == voterId);
    }

    public async Task<BiometricData?> GetByRFIDAsync(string rfidCardNumber)
    {
        return await _dbSet
            .Include(b => b.Voter)
            .ThenInclude(v => v.User)
            .FirstOrDefaultAsync(b => b.RFIDCardNumber == rfidCardNumber && b.IsActive);
    }

    public async Task<bool> VerifyFingerprintAsync(string fingerprintTemplate)
    {
        return await _dbSet
            .AnyAsync(b => b.FingerprintTemplate == fingerprintTemplate && b.IsActive);
    }
}
