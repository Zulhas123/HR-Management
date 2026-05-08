using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface ILeaveBalanceRepository : IRepository<LeaveBalance>
{
    Task<LeaveBalance?> GetByEmployeeLeaveTypeYearAsync(
        int employeeId,
        int leaveTypeId,
        int year,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveBalance>> ListByEmployeeYearAsync(
        int employeeId,
        int year,
        CancellationToken cancellationToken = default);
}

