using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface IEmployeeTaskRepository : IRepository<EmployeeTask>
{
    Task<IReadOnlyList<EmployeeTask>> ListByAssignedDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default);
}
