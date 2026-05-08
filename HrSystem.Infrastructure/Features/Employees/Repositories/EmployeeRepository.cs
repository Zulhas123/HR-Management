using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    private readonly HrSystemDbContext _dbContext;

    public EmployeeRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<Employee> Query() =>
        _dbContext.Employees
            .Include(x => x.Department)
            .Include(x => x.Designation)
            .Include(x => x.EmploymentType)
            .Include(x => x.Religion)
            .Include(x => x.BloodGroup);

    public Task<Employee?> GetByBiometricUserIdAsync(string biometricUserId, CancellationToken cancellationToken = default) =>
        _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.BiometricUserId == biometricUserId, cancellationToken);

    public Task<Employee?> GetByFaceProfileIdAsync(string faceProfileId, CancellationToken cancellationToken = default) =>
        _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.FaceProfileId == faceProfileId, cancellationToken);

    public Task<Employee?> GetByRfidCardIdAsync(string rfidCardId, CancellationToken cancellationToken = default) =>
        _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.RfidCardId == rfidCardId, cancellationToken);
}
