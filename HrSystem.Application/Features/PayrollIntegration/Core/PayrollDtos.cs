using HrSystem.Domain.Entities;

namespace HrSystem.Application.PayrollIntegration;

public sealed record PayrollBonusLineDto(
    int Id,
    int EmployeeId,
    DateOnly AwardDate,
    decimal Amount,
    string? Title,
    string? Notes,
    DateTimeOffset? SyncedAtUtc);

public sealed record PayrollSalaryAdjustmentLineDto(
    int Id,
    int EmployeeId,
    DateOnly EffectiveDate,
    SalaryAdjustmentKind Kind,
    decimal Amount,
    string? Reason,
    DateTimeOffset? SyncedAtUtc);

public sealed record PayrollEmployeeSummaryDto(
    int EmployeeId,
    string EmployeeCode,
    string FullName,
    int AttendanceDays,
    int MissingPunchDays,
    int WorkedMinutes,
    int LateMinutes,
    int EarlyExitMinutes,
    int OvertimeMinutes,
    decimal PaidLeaveDays,
    decimal UnpaidLeaveDays,
    decimal BonusTotal,
    IReadOnlyList<PayrollBonusLineDto> Bonuses,
    IReadOnlyList<PayrollSalaryAdjustmentLineDto> SalaryAdjustments);

public sealed record PayrollPeriodSummaryDto(
    DateOnly FromInclusive,
    DateOnly ToInclusive,
    IReadOnlyList<PayrollEmployeeSummaryDto> Employees);

public sealed record PayrollPeriodExportDto(
    DateOnly FromInclusive,
    DateOnly ToInclusive,
    bool OnlyUnsyncedBonusesAndAdjustments,
    bool MarkedBonusesAndAdjustmentsAsSynced,
    DateTimeOffset GeneratedAtUtc,
    IReadOnlyList<PayrollEmployeeSummaryDto> Employees);
