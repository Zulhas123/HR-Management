using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByBiometricUserIdAsync(string biometricUserId, CancellationToken cancellationToken = default);
    Task<Employee?> GetByFaceProfileIdAsync(string faceProfileId, CancellationToken cancellationToken = default);
    Task<Employee?> GetByRfidCardIdAsync(string rfidCardId, CancellationToken cancellationToken = default);
}

