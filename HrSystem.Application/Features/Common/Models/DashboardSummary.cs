namespace HrSystem.Application.Features.Common.Models;

public sealed record DashboardSummary(
    DateOnly Today,
    int TotalEmployees,
    int AttendanceToday,
    int PendingLeaveRequests,
    int PendingOvertimeRequests,
    int OpenJobPostings,
    DateTimeOffset GeneratedAtUtc);

