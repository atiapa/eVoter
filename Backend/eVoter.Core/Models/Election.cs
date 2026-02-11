using eVoter.Core.Enums;

namespace eVoter.Core.Models;

public class Election
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ElectionStatus Status { get; set; } = ElectionStatus.Draft;
    public bool AllowMobileVoting { get; set; } = true;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public virtual ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
