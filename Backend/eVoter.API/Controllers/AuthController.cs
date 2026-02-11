using Microsoft.AspNetCore.Mvc;
using eVoter.Core.Interfaces;
using eVoter.Core.Models;
using eVoter.Core.Enums;
using eVoter.API.DTOs;
using eVoter.API.Services;

namespace eVoter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IVoterRepository _voterRepository;
    private readonly IBiometricDataRepository _biometricRepository;
    private readonly IAuthService _authService;

    public AuthController(
        IUserRepository userRepository,
        IVoterRepository voterRepository,
        IBiometricDataRepository biometricRepository,
        IAuthService authService)
    {
        _userRepository = userRepository;
        _voterRepository = voterRepository;
        _biometricRepository = biometricRepository;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        User? user = null;

        // Check for RFID authentication
        if (!string.IsNullOrEmpty(request.RFIDCardNumber))
        {
            var biometric = await _biometricRepository.GetByRFIDAsync(request.RFIDCardNumber);
            if (biometric != null)
            {
                user = biometric.Voter.User;
            }
        }
        // Check for fingerprint authentication
        else if (!string.IsNullOrEmpty(request.FingerprintTemplate))
        {
            if (await _biometricRepository.VerifyFingerprintAsync(request.FingerprintTemplate))
            {
                user = await _userRepository.GetByUsernameAsync(request.Username);
            }
        }
        // Standard username/password authentication
        else
        {
            user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user != null && !_authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = _authService.GenerateJwtToken(user);
        var voter = await _voterRepository.GetByUserIdAsync(user.Id);

        return Ok(new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            UserId = user.Id,
            VoterId = voter?.Id
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterVoterRequest request)
    {
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        var existingVoter = await _voterRepository.GetByNationalIdAsync(request.NationalId);
        if (existingVoter != null)
        {
            return BadRequest(new { message = "National ID already registered" });
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _authService.HashPassword(request.Password),
            Role = UserRole.Voter,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user = await _userRepository.AddAsync(user);

        var voter = new Voter
        {
            UserId = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            NationalId = request.NationalId,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            IsVerified = false,
            RegisteredAt = DateTime.UtcNow
        };

        await _voterRepository.AddAsync(voter);

        return Ok(new { message = "Registration successful", userId = user.Id });
    }

    [HttpPost("register-biometric")]
    public async Task<ActionResult> RegisterBiometric([FromBody] BiometricRegistrationRequest request)
    {
        var voter = await _voterRepository.GetByIdAsync(request.VoterId);
        if (voter == null)
        {
            return NotFound(new { message = "Voter not found" });
        }

        var existingBiometric = await _biometricRepository.GetByVoterIdAsync(request.VoterId);
        if (existingBiometric != null)
        {
            return BadRequest(new { message = "Biometric data already registered" });
        }

        if (!string.IsNullOrEmpty(request.RFIDCardNumber))
        {
            var existingRFID = await _biometricRepository.GetByRFIDAsync(request.RFIDCardNumber);
            if (existingRFID != null)
            {
                return BadRequest(new { message = "RFID card already registered" });
            }
        }

        var biometricData = new BiometricData
        {
            VoterId = request.VoterId,
            FingerprintTemplate = request.FingerprintTemplate,
            RFIDCardNumber = request.RFIDCardNumber,
            AuthType = Enum.Parse<AuthenticationType>(request.AuthType),
            IsActive = true,
            RegisteredAt = DateTime.UtcNow
        };

        await _biometricRepository.AddAsync(biometricData);

        return Ok(new { message = "Biometric registration successful" });
    }
}
