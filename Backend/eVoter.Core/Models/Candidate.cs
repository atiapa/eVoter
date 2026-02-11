namespace eVoter.Core.Models;

public class Candidate
{
    public int Id { get; set; }
    public int ElectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhotoPath { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Election Election { get; set; } = null!;
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
