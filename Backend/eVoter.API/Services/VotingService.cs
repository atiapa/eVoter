using eVoter.Core.Models;
using eVoter.Core.Interfaces;
using eVoter.Core.Enums;
using eVoter.API.DTOs;

namespace eVoter.API.Services;

public interface IVotingService
{
    Task<bool> CastVoteAsync(int voterId, VoteRequest request, string? ipAddress, string? deviceInfo);
    Task<bool> HasVotedAsync(int voterId, int electionId);
    Task<IEnumerable<Election>> GetActiveElectionsAsync();
    Task<Election?> GetElectionAsync(int id);
}

public class VotingService : IVotingService
{
    private readonly IVoterRepository _voterRepository;
    private readonly IElectionRepository _electionRepository;
    private readonly eVoter.Data.Context.eVoterDbContext _context;

    public VotingService(
        IVoterRepository voterRepository,
        IElectionRepository electionRepository,
        eVoter.Data.Context.eVoterDbContext context)
    {
        _voterRepository = voterRepository;
        _electionRepository = electionRepository;
        _context = context;
    }

    public async Task<bool> CastVoteAsync(int voterId, VoteRequest request, string? ipAddress, string? deviceInfo)
    {
        var election = await _electionRepository.GetElectionWithCandidatesAsync(request.ElectionId);
        if (election == null || election.Status != ElectionStatus.Active)
            return false;

        if (await _voterRepository.HasVotedInElectionAsync(voterId, request.ElectionId))
            return false;

        var vote = new Vote
        {
            ElectionId = request.ElectionId,
            VoterId = voterId,
            CandidateId = request.CandidateId,
            VotedAt = DateTime.UtcNow,
            Status = VoteStatus.Confirmed,
            IsFromMobile = request.IsFromMobile,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo,
            EncryptedVoteHash = GenerateVoteHash(voterId, request.ElectionId, request.CandidateId)
        };

        await _context.Votes.AddAsync(vote);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> HasVotedAsync(int voterId, int electionId)
    {
        return await _voterRepository.HasVotedInElectionAsync(voterId, electionId);
    }

    public async Task<IEnumerable<Election>> GetActiveElectionsAsync()
    {
        return await _electionRepository.GetActiveElectionsAsync();
    }

    public async Task<Election?> GetElectionAsync(int id)
    {
        return await _electionRepository.GetElectionWithCandidatesAsync(id);
    }

    private string GenerateVoteHash(int voterId, int electionId, int candidateId)
    {
        var data = $"{voterId}-{electionId}-{candidateId}-{DateTime.UtcNow.Ticks}";
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }
}
