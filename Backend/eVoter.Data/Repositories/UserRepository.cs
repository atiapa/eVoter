using Microsoft.EntityFrameworkCore;
using eVoter.Core.Models;
using eVoter.Core.Interfaces;
using eVoter.Data.Context;

namespace eVoter.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(eVoterDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(u => u.Voter)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Voter)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }
}
