using HrSystem.Application.Abstractions;
using HrSystem.Application.Attendance;
using HrSystem.Application.PayrollIntegration;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class PayrollIntegrationService(
    IEmployeeRepository employees,
    IAttendanceRecordRepository attendance,
    ILeaveRequestRepository leaveRequests,
    IEmployeeBonusRepository bonuses,
    ISalaryAdjustmentRepository salaryAdjustments,
    ILeaveCalendarService leaveCalendar) : IPayrollIntegrationService
{
    public async Task<PayrollPeriodSummaryDto> GetPeriodSummaryAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default)
    {
        var export = await ExportPeriodAsync(
            fromInclusive,
            toInclusive,
            onlyUnsyncedBonusesAndAdjustments: false,
            markBonusesAndAdjustmentsAsSynced: false,
            cancellationToken);

        return new PayrollPeriodSummaryDto(export.FromInclusive, export.ToInclusive, export.Employees);
    }

    public async Task<PayrollPeriodExportDto> ExportPeriodAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool onlyUnsyncedBonusesAndAdjustments = true,
        bool markBonusesAndAdjustmentsAsSynced = false,
        CancellationToken cancellationToken = default)
    {
        if (toInclusive < fromInclusive)
        {
            throw new ArgumentException("toInclusive must be >= fromInclusive");
        }

        var employeeList = await employees.ListAsync(cancellationToken);
        var eligibleEmployees = employeeList
            .Where(e => e.JoinDate <= toInclusive && (e.ResignationDate is null || e.ResignationDate.Value >= fromInclusive))
            .ToList();

        var attendanceRecords = await attendance.ListByDateRangeAsync(fromInclusive, toInclusive, cancellationToken);
        var approvedLeaves = await leaveRequests.ListApprovedOverlappingDateRangeAsync(fromInclusive, toInclusive, cancellationToken);

        var bonusList = await bonuses.ListByAwardDateRangeAsync(
            fromInclusive,
            toInclusive,
            onlyUnsynced: onlyUnsyncedBonusesAndAdjustments,
            cancellationToken);

        var adjustmentList = await salaryAdjustments.ListByEffectiveDateRangeAsync(
            fromInclusive,
            toInclusive,
            onlyUnsynced: onlyUnsyncedBonusesAndAdjustments,
            cancellationToken);

        var employeeById = eligibleEmployees.ToDictionary(x => x.Id);
        var summaries = new Dictionary<int, MutableEmployeeSummary>();

        MutableEmployeeSummary GetOrCreate(int employeeId)
        {
            if (!employeeById.TryGetValue(employeeId, out var employee))
            {
                // Skip employees outside the eligible join/resign window.
                employee = employeeList.FirstOrDefault(x => x.Id == employeeId);
                if (employee is null)
                {
                    throw new InvalidOperationException($"Employee {employeeId} not found.");
                }
            }

            if (!summaries.TryGetValue(employeeId, out var s))
            {
                s = new MutableEmployeeSummary(employeeId, employee.EmployeeCode, $"{employee.FirstName} {employee.LastName}".Trim());
                summaries.Add(employeeId, s);
            }

            return s;
        }

        // Attendance -> payroll
        foreach (var record in attendanceRecords)
        {
            if (!employeeById.ContainsKey(record.EmployeeId))
            {
                continue;
            }

            var s = GetOrCreate(record.EmployeeId);
            s.AttendanceDays += 1;

            var missingPunchStatus = record.MissingPunchStatus;
            if (missingPunchStatus == MissingPunchStatus.None && (record.CheckInTime is null || record.CheckOutTime is null))
            {
                missingPunchStatus = record.CheckInTime is null && record.CheckOutTime is null
                    ? MissingPunchStatus.MissingBoth
                    : record.CheckInTime is null
                        ? MissingPunchStatus.MissingCheckIn
                        : MissingPunchStatus.MissingCheckOut;
            }

            if (missingPunchStatus != MissingPunchStatus.None)
            {
                s.MissingPunchDays += 1;
            }

            var workedMinutes = record.WorkedMinutes;
            var lateMinutes = record.LateMinutes;
            var earlyExitMinutes = record.EarlyExitMinutes;

            if (!workedMinutes.HasValue && record.Shift is not null && record.CheckInTime is not null && record.CheckOutTime is not null)
            {
                workedMinutes = AttendanceMetrics.ComputeWorkedMinutes(record.CheckInTime.Value, record.CheckOutTime.Value, record.Shift.IsOvernight);
                lateMinutes = AttendanceMetrics.ComputeLateMinutes(record.CheckInTime.Value, record.Shift);
                earlyExitMinutes = AttendanceMetrics.ComputeEarlyExitMinutes(record.CheckOutTime.Value, record.Shift, workedMinutes.Value);
            }

            s.WorkedMinutes += workedMinutes ?? 0;
            s.LateMinutes += lateMinutes ?? 0;
            s.EarlyExitMinutes += earlyExitMinutes ?? 0;

            if (workedMinutes.HasValue && record.Shift?.RequiredWorkMinutes is int required && required > 0)
            {
                s.OvertimeMinutes += Math.Max(0, workedMinutes.Value - required);
            }
        }

        // Leave deduction integration
        foreach (var leave in approvedLeaves)
        {
            if (!employeeById.ContainsKey(leave.EmployeeId))
            {
                continue;
            }

            if (leave.LeaveType is null)
            {
                continue;
            }

            var overlapStart = leave.StartDate < fromInclusive ? fromInclusive : leave.StartDate;
            var overlapEnd = leave.EndDate > toInclusive ? toInclusive : leave.EndDate;
            if (overlapEnd < overlapStart)
            {
                continue;
            }

            var days = await leaveCalendar.CalculateChargeableDaysAsync(overlapStart, overlapEnd, leave.LeaveType, cancellationToken);
            if (days <= 0)
            {
                continue;
            }

            var s = GetOrCreate(leave.EmployeeId);
            if (leave.LeaveType.IsPaid)
            {
                s.PaidLeaveDays += days;
            }
            else
            {
                s.UnpaidLeaveDays += days;
            }
        }

        // Bonus integration
        foreach (var bonus in bonusList)
        {
            if (!employeeById.ContainsKey(bonus.EmployeeId))
            {
                continue;
            }

            var s = GetOrCreate(bonus.EmployeeId);
            s.BonusTotal += bonus.Amount;
            s.Bonuses.Add(new PayrollBonusLineDto(
                bonus.Id,
                bonus.EmployeeId,
                bonus.AwardDate,
                bonus.Amount,
                bonus.Title,
                bonus.Notes,
                bonus.SyncedAtUtc));
        }

        // Salary adjustment sync
        foreach (var adj in adjustmentList)
        {
            if (!employeeById.ContainsKey(adj.EmployeeId))
            {
                continue;
            }

            var s = GetOrCreate(adj.EmployeeId);
            s.SalaryAdjustments.Add(new PayrollSalaryAdjustmentLineDto(
                adj.Id,
                adj.EmployeeId,
                adj.EffectiveDate,
                adj.Kind,
                adj.Amount,
                adj.Reason,
                adj.SyncedAtUtc));
        }

        // Ensure all eligible employees are present in the export, even with zero activity.
        foreach (var emp in eligibleEmployees)
        {
            _ = GetOrCreate(emp.Id);
        }

        var resultEmployees = summaries.Values
            .OrderBy(x => x.EmployeeCode)
            .Select(x => x.ToDto())
            .ToList();

        var generatedAtUtc = DateTimeOffset.UtcNow;

        if (markBonusesAndAdjustmentsAsSynced)
        {
            await MarkBonusesAsSyncedAsync(resultEmployees, generatedAtUtc, cancellationToken);
            await MarkAdjustmentsAsSyncedAsync(resultEmployees, generatedAtUtc, cancellationToken);
        }

        return new PayrollPeriodExportDto(
            fromInclusive,
            toInclusive,
            onlyUnsyncedBonusesAndAdjustments,
            markBonusesAndAdjustmentsAsSynced,
            generatedAtUtc,
            resultEmployees);
    }

    private async Task MarkBonusesAsSyncedAsync(
        IReadOnlyList<PayrollEmployeeSummaryDto> employeesToExport,
        DateTimeOffset syncedAtUtc,
        CancellationToken cancellationToken)
    {
        var ids = employeesToExport.SelectMany(x => x.Bonuses).Where(x => x.SyncedAtUtc is null).Select(x => x.Id).Distinct().ToList();
        if (ids.Count == 0)
        {
            return;
        }

        foreach (var id in ids)
        {
            var entity = await bonuses.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.SyncedAtUtc is not null)
            {
                continue;
            }

            entity.SyncedAtUtc = syncedAtUtc;
            await bonuses.UpdateAsync(entity, cancellationToken);
        }

        await bonuses.SaveChangesAsync(cancellationToken);
    }

    private async Task MarkAdjustmentsAsSyncedAsync(
        IReadOnlyList<PayrollEmployeeSummaryDto> employeesToExport,
        DateTimeOffset syncedAtUtc,
        CancellationToken cancellationToken)
    {
        var ids = employeesToExport.SelectMany(x => x.SalaryAdjustments).Where(x => x.SyncedAtUtc is null).Select(x => x.Id).Distinct().ToList();
        if (ids.Count == 0)
        {
            return;
        }

        foreach (var id in ids)
        {
            var entity = await salaryAdjustments.GetByIdAsync(id, cancellationToken);
            if (entity is null || entity.SyncedAtUtc is not null)
            {
                continue;
            }

            entity.SyncedAtUtc = syncedAtUtc;
            await salaryAdjustments.UpdateAsync(entity, cancellationToken);
        }

        await salaryAdjustments.SaveChangesAsync(cancellationToken);
    }

    private sealed class MutableEmployeeSummary(int employeeId, string employeeCode, string fullName)
    {
        public int EmployeeId { get; } = employeeId;
        public string EmployeeCode { get; } = employeeCode;
        public string FullName { get; } = fullName;

        public int AttendanceDays { get; set; }
        public int MissingPunchDays { get; set; }
        public int WorkedMinutes { get; set; }
        public int LateMinutes { get; set; }
        public int EarlyExitMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
        public decimal PaidLeaveDays { get; set; }
        public decimal UnpaidLeaveDays { get; set; }
        public decimal BonusTotal { get; set; }

        public List<PayrollBonusLineDto> Bonuses { get; } = [];
        public List<PayrollSalaryAdjustmentLineDto> SalaryAdjustments { get; } = [];

        public PayrollEmployeeSummaryDto ToDto() =>
            new(
                EmployeeId,
                EmployeeCode,
                FullName,
                AttendanceDays,
                MissingPunchDays,
                WorkedMinutes,
                LateMinutes,
                EarlyExitMinutes,
                OvertimeMinutes,
                PaidLeaveDays,
                UnpaidLeaveDays,
                BonusTotal,
                Bonuses.OrderBy(x => x.AwardDate).ThenBy(x => x.Id).ToList(),
                SalaryAdjustments.OrderBy(x => x.EffectiveDate).ThenBy(x => x.Id).ToList());
    }
}
