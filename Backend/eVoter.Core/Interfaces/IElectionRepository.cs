using eVoter.Core.Models;
using eVoter.Core.Enums;

namespace eVoter.Core.Interfaces;

public interface IElectionRepository : IRepository<Election>
{
    Task<IEnumerable<Election>> GetActiveElectionsAsync();
    Task<IEnumerable<Election>> GetElectionsByStatusAsync(ElectionStatus status);
    Task<Election?> GetElectionWithCandidatesAsync(int id);
}
