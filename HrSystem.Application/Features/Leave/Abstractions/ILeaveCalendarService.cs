using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions;

public interface ILeaveCalendarService
{
    Task<decimal> CalculateChargeableDaysAsync(
        DateOnly startDate,
        DateOnly endDate,
        LeaveType leaveType,
        CancellationToken cancellationToken = default);

    Task<bool> IsWeekendAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<bool> IsHolidayAsync(DateOnly date, CancellationToken cancellationToken = default);
}
