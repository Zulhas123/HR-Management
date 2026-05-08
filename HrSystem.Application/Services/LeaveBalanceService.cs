using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class LeaveBalanceService(
    ILeaveBalanceRepository balances,
    IRepository<LeaveType> leaveTypes) : ILeaveBalanceService
{
    public async Task<LeaveBalance> GetOrCreateAsync(int employeeId, int leaveTypeId, int year, CancellationToken cancellationToken = default)
    {
        var existing = await balances.GetByEmployeeLeaveTypeYearAsync(employeeId, leaveTypeId, year, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var leaveType = await leaveTypes.GetByIdAsync(leaveTypeId, cancellationToken);
        if (leaveType is null)
        {
            throw new InvalidOperationException("Leave type not found.");
        }

        var created = new LeaveBalance
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveTypeId,
            Year = year,
            AllocatedDays = leaveType.DefaultAnnualAllocation,
            UsedDays = 0,
            EncashmentDays = 0,
            LastRecalculatedAtUtc = DateTimeOffset.UtcNow
        };

        await balances.AddAsync(created, cancellationToken);
        await balances.SaveChangesAsync(cancellationToken);
        return created;
    }

    public async Task<decimal?> GetRemainingDaysAsync(int employeeId, LeaveType leaveType, int year, CancellationToken cancellationToken = default)
    {
        // For unpaid leave types (e.g. Leave Without Pay), treat balance as unlimited in MVP.
        if (!leaveType.IsPaid)
        {
            return null;
        }

        var balance = await GetOrCreateAsync(employeeId, leaveType.Id, year, cancellationToken);
        return Math.Max(0, balance.AllocatedDays - balance.UsedDays - balance.EncashmentDays);
    }

    public async Task ApplyApprovedLeaveAsync(LeaveRequest request, CancellationToken cancellationToken = default)
    {
        var leaveType = request.LeaveType;
        if (leaveType is null)
        {
            leaveType = await leaveTypes.GetByIdAsync(request.LeaveTypeId, cancellationToken);
        }

        if (leaveType is null)
        {
            throw new InvalidOperationException("Leave type not found.");
        }

        if (!leaveType.IsPaid)
        {
            return;
        }

        var year = request.StartDate.Year;
        var balance = await GetOrCreateAsync(request.EmployeeId, request.LeaveTypeId, year, cancellationToken);

        var remaining = Math.Max(0, balance.AllocatedDays - balance.UsedDays - balance.EncashmentDays);
        if (request.TotalDays > remaining)
        {
            throw new InvalidOperationException($"Insufficient leave balance. Remaining={remaining}, Requested={request.TotalDays}");
        }

        balance.UsedDays += request.TotalDays;
        balance.LastRecalculatedAtUtc = DateTimeOffset.UtcNow;
        await balances.UpdateAsync(balance, cancellationToken);
        await balances.SaveChangesAsync(cancellationToken);
    }

    public async Task ApplyEncashmentApprovedAsync(LeaveEncashmentRequest request, CancellationToken cancellationToken = default)
    {
        var leaveType = request.LeaveType;
        if (leaveType is null)
        {
            leaveType = await leaveTypes.GetByIdAsync(request.LeaveTypeId, cancellationToken);
        }

        if (leaveType is null)
        {
            throw new InvalidOperationException("Leave type not found.");
        }

        if (!leaveType.AllowEncashment)
        {
            throw new InvalidOperationException("Encashment is not allowed for this leave type.");
        }

        var balance = await GetOrCreateAsync(request.EmployeeId, request.LeaveTypeId, request.Year, cancellationToken);
        var remaining = Math.Max(0, balance.AllocatedDays - balance.UsedDays - balance.EncashmentDays);
        if (request.DaysRequested > remaining)
        {
            throw new InvalidOperationException($"Insufficient leave balance. Remaining={remaining}, Requested={request.DaysRequested}");
        }

        balance.EncashmentDays += request.DaysRequested;
        balance.LastRecalculatedAtUtc = DateTimeOffset.UtcNow;
        await balances.UpdateAsync(balance, cancellationToken);
        await balances.SaveChangesAsync(cancellationToken);
    }
}

