namespace eVoter.Core.Models;

public class Voter
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    
    public virtual User User { get; set; } = null!;
    public virtual BiometricData? BiometricData { get; set; }
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
