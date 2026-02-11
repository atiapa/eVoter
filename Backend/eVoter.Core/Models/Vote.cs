using eVoter.Core.Enums;

namespace eVoter.Core.Models;

public class Vote
{
    public int Id { get; set; }
    public int ElectionId { get; set; }
    public int VoterId { get; set; }
    public int CandidateId { get; set; }
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    public VoteStatus Status { get; set; } = VoteStatus.Confirmed;
    public string? EncryptedVoteHash { get; set; }
    public bool IsFromMobile { get; set; } = false;
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    
    public virtual Election Election { get; set; } = null!;
    public virtual Voter Voter { get; set; } = null!;
    public virtual Candidate Candidate { get; set; } = null!;
}
