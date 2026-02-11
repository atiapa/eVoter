using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using eVoter.API.DTOs;
using eVoter.API.Services;

namespace eVoter.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VotingController : ControllerBase
{
    private readonly IVotingService _votingService;

    public VotingController(IVotingService votingService)
    {
        _votingService = votingService;
    }

    [HttpGet("elections")]
    public async Task<ActionResult<IEnumerable<ElectionDto>>> GetActiveElections()
    {
        var elections = await _votingService.GetActiveElectionsAsync();
        var electionDtos = elections.Select(e => new ElectionDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Status = e.Status.ToString(),
            AllowMobileVoting = e.AllowMobileVoting,
            Candidates = e.Candidates.Select(c => new CandidateDto
            {
                Id = c.Id,
                Name = c.Name,
                Party = c.Party,
                Description = c.Description,
                PhotoPath = c.PhotoPath
            }).ToList()
        });

        return Ok(electionDtos);
    }

    [HttpGet("elections/{id}")]
    public async Task<ActionResult<ElectionDto>> GetElection(int id)
    {
        var election = await _votingService.GetElectionAsync(id);
        if (election == null)
        {
            return NotFound(new { message = "Election not found" });
        }

        var electionDto = new ElectionDto
        {
            Id = election.Id,
            Title = election.Title,
            Description = election.Description,
            StartDate = election.StartDate,
            EndDate = election.EndDate,
            Status = election.Status.ToString(),
            AllowMobileVoting = election.AllowMobileVoting,
            Candidates = election.Candidates.Select(c => new CandidateDto
            {
                Id = c.Id,
                Name = c.Name,
                Party = c.Party,
                Description = c.Description,
                PhotoPath = c.PhotoPath
            }).ToList()
        };

        return Ok(electionDto);
    }

    [HttpPost("vote")]
    public async Task<ActionResult> CastVote([FromBody] VoteRequest request)
    {
        var voterIdClaim = User.FindFirst("VoterId")?.Value;
        if (string.IsNullOrEmpty(voterIdClaim))
        {
            return BadRequest(new { message = "Voter information not found" });
        }

        var voterId = int.Parse(voterIdClaim);
        
        if (await _votingService.HasVotedAsync(voterId, request.ElectionId))
        {
            return BadRequest(new { message = "You have already voted in this election" });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        var success = await _votingService.CastVoteAsync(voterId, request, ipAddress, userAgent);
        if (!success)
        {
            return BadRequest(new { message = "Failed to cast vote" });
        }

        return Ok(new { message = "Vote cast successfully" });
    }

    [HttpGet("has-voted/{electionId}")]
    public async Task<ActionResult<bool>> HasVoted(int electionId)
    {
        var voterIdClaim = User.FindFirst("VoterId")?.Value;
        if (string.IsNullOrEmpty(voterIdClaim))
        {
            return BadRequest(new { message = "Voter information not found" });
        }

        var voterId = int.Parse(voterIdClaim);
        var hasVoted = await _votingService.HasVotedAsync(voterId, electionId);

        return Ok(hasVoted);
    }
}
