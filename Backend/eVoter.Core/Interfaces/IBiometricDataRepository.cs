using eVoter.Core.Models;

namespace eVoter.Core.Interfaces;

public interface IBiometricDataRepository : IRepository<BiometricData>
{
    Task<BiometricData?> GetByVoterIdAsync(int voterId);
    Task<BiometricData?> GetByRFIDAsync(string rfidCardNumber);
    Task<bool> VerifyFingerprintAsync(string fingerprintTemplate);
}
