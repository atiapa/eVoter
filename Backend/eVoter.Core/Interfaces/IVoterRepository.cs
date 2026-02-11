using eVoter.Core.Models;

namespace eVoter.Core.Interfaces;

public interface IVoterRepository : IRepository<Voter>
{
    Task<Voter?> GetByUserIdAsync(int userId);
    Task<Voter?> GetByNationalIdAsync(string nationalId);
    Task<bool> HasVotedInElectionAsync(int voterId, int electionId);
}
