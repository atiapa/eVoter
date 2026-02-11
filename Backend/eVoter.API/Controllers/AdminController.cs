using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eVoter.Core.Interfaces;
using eVoter.Core.Models;
using eVoter.Core.Enums;
using eVoter.API.DTOs;

namespace eVoter.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IElectionRepository _electionRepository;
    private readonly IVoterRepository _voterRepository;
    private readonly eVoter.Data.Context.eVoterDbContext _context;

    public AdminController(
        IElectionRepository electionRepository,
        IVoterRepository voterRepository,
        eVoter.Data.Context.eVoterDbContext context)
    {
        _electionRepository = electionRepository;
        _voterRepository = voterRepository;
        _context = context;
    }

    [HttpPost("elections")]
    public async Task<ActionResult> CreateElection([FromBody] CreateElectionRequest request)
    {
        var election = new Election
        {
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = ElectionStatus.Draft,
            AllowMobileVoting = request.AllowMobileVoting,
            CreatedByUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0"),
            CreatedAt = DateTime.UtcNow
        };

        election = await _electionRepository.AddAsync(election);

        var displayOrder = 1;
        foreach (var candidateRequest in request.Candidates)
        {
            var candidate = new Candidate
            {
                ElectionId = election.Id,
                Name = candidateRequest.Name,
                Party = candidateRequest.Party,
                Description = candidateRequest.Description,
                DisplayOrder = displayOrder++,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Candidates.AddAsync(candidate);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Election created successfully", electionId = election.Id });
    }

    [HttpPut("elections/{id}/status")]
    public async Task<ActionResult> UpdateElectionStatus(int id, [FromBody] string status)
    {
        var election = await _electionRepository.GetByIdAsync(id);
        if (election == null)
        {
            return NotFound(new { message = "Election not found" });
        }

        if (Enum.TryParse<ElectionStatus>(status, out var electionStatus))
        {
            election.Status = electionStatus;
            election.UpdatedAt = DateTime.UtcNow;
            await _electionRepository.UpdateAsync(election);

            return Ok(new { message = "Election status updated successfully" });
        }

        return BadRequest(new { message = "Invalid status value" });
    }

    [HttpGet("elections")]
    public async Task<ActionResult<IEnumerable<ElectionDto>>> GetAllElections()
    {
        var elections = await _electionRepository.GetAllAsync();
        var electionDtos = elections.Select(e => new ElectionDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Status = e.Status.ToString(),
            AllowMobileVoting = e.AllowMobileVoting
        });

        return Ok(electionDtos);
    }

    [HttpGet("voters")]
    public async Task<ActionResult<IEnumerable<VoterDto>>> GetAllVoters()
    {
        var voters = await _voterRepository.GetAllAsync();
        var voterDtos = voters.Select(v => new VoterDto
        {
            Id = v.Id,
            FirstName = v.FirstName,
            LastName = v.LastName,
            NationalId = v.NationalId,
            Email = v.User.Email,
            IsVerified = v.IsVerified,
            HasBiometric = v.BiometricData != null
        });

        return Ok(voterDtos);
    }

    [HttpPut("voters/{id}/verify")]
    public async Task<ActionResult> VerifyVoter(int id)
    {
        var voter = await _voterRepository.GetByIdAsync(id);
        if (voter == null)
        {
            return NotFound(new { message = "Voter not found" });
        }

        voter.IsVerified = true;
        await _voterRepository.UpdateAsync(voter);

        return Ok(new { message = "Voter verified successfully" });
    }

    [HttpGet("elections/{id}/results")]
    public async Task<ActionResult> GetElectionResults(int id)
    {
        var election = await _electionRepository.GetElectionWithCandidatesAsync(id);
        if (election == null)
        {
            return NotFound(new { message = "Election not found" });
        }

        var results = election.Candidates.Select(c => new
        {
            CandidateId = c.Id,
            Name = c.Name,
            Party = c.Party,
            VoteCount = c.Votes.Count
        }).OrderByDescending(r => r.VoteCount);

        return Ok(new
        {
            ElectionId = election.Id,
            Title = election.Title,
            TotalVotes = election.Votes.Count,
            Results = results
        });
    }
}
