using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions;

public interface ILeaveBalanceService
{
    Task<LeaveBalance> GetOrCreateAsync(int employeeId, int leaveTypeId, int year, CancellationToken cancellationToken = default);
    Task<decimal?> GetRemainingDaysAsync(int employeeId, LeaveType leaveType, int year, CancellationToken cancellationToken = default);
    Task ApplyApprovedLeaveAsync(LeaveRequest request, CancellationToken cancellationToken = default);
    Task ApplyEncashmentApprovedAsync(LeaveEncashmentRequest request, CancellationToken cancellationToken = default);
}

