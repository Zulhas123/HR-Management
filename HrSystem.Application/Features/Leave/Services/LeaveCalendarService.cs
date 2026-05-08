using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class LeaveCalendarService(
    IHolidayRepository holidays,
    IRepository<WeekendConfiguration> weekends) : ILeaveCalendarService
{
    public async Task<decimal> CalculateChargeableDaysAsync(
        DateOnly startDate,
        DateOnly endDate,
        LeaveType leaveType,
        CancellationToken cancellationToken = default)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException("endDate must be >= startDate");
        }

        var weekendConfig = await GetWeekendConfigurationAsync(cancellationToken);
        var holidayDates = (await holidays.ListByDateRangeAsync(startDate, endDate, cancellationToken))
            .Select(h => h.Date)
            .ToHashSet();

        decimal total = 0;
        for (var day = startDate; day <= endDate; day = day.AddDays(1))
        {
            var isWeekend = IsWeekend(day, weekendConfig);
            var isHoliday = holidayDates.Contains(day);

            if (!leaveType.CountWeekendsAsLeave && isWeekend)
            {
                continue;
            }

            if (!leaveType.CountHolidaysAsLeave && isHoliday)
            {
                continue;
            }

            total += 1;
        }

        return total;
    }

    public async Task<bool> IsWeekendAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var weekendConfig = await GetWeekendConfigurationAsync(cancellationToken);
        return IsWeekend(date, weekendConfig);
    }

    public async Task<bool> IsHolidayAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var items = await holidays.ListByDateRangeAsync(date, date, cancellationToken);
        return items.Count > 0;
    }

    private async Task<WeekendConfiguration> GetWeekendConfigurationAsync(CancellationToken cancellationToken)
    {
        var configs = await weekends.ListAsync(cancellationToken);
        return configs.FirstOrDefault() ?? new WeekendConfiguration { Friday = true, Saturday = true };
    }

    private static bool IsWeekend(DateOnly date, WeekendConfiguration config)
    {
        var dow = date.DayOfWeek;
        return dow switch
        {
            DayOfWeek.Sunday => config.Sunday,
            DayOfWeek.Monday => config.Monday,
            DayOfWeek.Tuesday => config.Tuesday,
            DayOfWeek.Wednesday => config.Wednesday,
            DayOfWeek.Thursday => config.Thursday,
            DayOfWeek.Friday => config.Friday,
            DayOfWeek.Saturday => config.Saturday,
            _ => false
        };
    }
}
