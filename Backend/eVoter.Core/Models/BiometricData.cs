using eVoter.Core.Enums;

namespace eVoter.Core.Models;

public class BiometricData
{
    public int Id { get; set; }
    public int VoterId { get; set; }
    public string? FingerprintTemplate { get; set; }
    public string? RFIDCardNumber { get; set; }
    public AuthenticationType AuthType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastVerifiedAt { get; set; }
    
    public virtual Voter Voter { get; set; } = null!;
}
