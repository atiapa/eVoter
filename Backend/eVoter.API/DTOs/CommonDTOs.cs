namespace eVoter.API.DTOs;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FingerprintTemplate { get; set; }
    public string? RFIDCardNumber { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int? VoterId { get; set; }
}

public class RegisterVoterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class BiometricRegistrationRequest
{
    public int VoterId { get; set; }
    public string? FingerprintTemplate { get; set; }
    public string? RFIDCardNumber { get; set; }
    public string AuthType { get; set; } = "Biometric";
}

public class VoteRequest
{
    public int ElectionId { get; set; }
    public int CandidateId { get; set; }
    public bool IsFromMobile { get; set; }
}

public class ElectionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool AllowMobileVoting { get; set; }
    public List<CandidateDto> Candidates { get; set; } = new();
}

public class CandidateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhotoPath { get; set; }
}

public class CreateElectionRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AllowMobileVoting { get; set; } = true;
    public List<CreateCandidateRequest> Candidates { get; set; } = new();
}

public class CreateCandidateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class VoterDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool HasBiometric { get; set; }
}
