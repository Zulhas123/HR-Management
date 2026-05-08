using HrSystem.Application.Features.Common.Abstractions;
using HrSystem.Application.Features.Common.Models;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Features.Common.Services;

public sealed class DashboardService(
    IRepository<Employee> employees,
    IRepository<AttendanceRecord> attendanceRecords,
    IRepository<LeaveRequest> leaveRequests,
    IRepository<OvertimeRequest> overtimeRequests,
    IRepository<JobPosting> jobPostings) : IDashboardService
{
    public async Task<DashboardSummary> GetSummaryAsync(DateOnly? today = null, CancellationToken cancellationToken = default)
    {
        var effectiveToday = today ?? DateOnly.FromDateTime(DateTime.Today);

        var totalEmployeesTask = employees.CountAsync(cancellationToken: cancellationToken);
        var attendanceTodayTask = attendanceRecords.CountAsync(x => x.Date == effectiveToday, cancellationToken);
        var pendingLeaveRequestsTask = leaveRequests.CountAsync(x => x.Status == LeaveRequestStatus.Pending, cancellationToken);
        var pendingOvertimeRequestsTask = overtimeRequests.CountAsync(x => x.Status == OvertimeRequestStatus.Pending, cancellationToken);
        var openJobPostingsTask = jobPostings.CountAsync(x => x.IsOpen, cancellationToken);

        await Task.WhenAll(
            totalEmployeesTask,
            attendanceTodayTask,
            pendingLeaveRequestsTask,
            pendingOvertimeRequestsTask,
            openJobPostingsTask);

        return new DashboardSummary(
            Today: effectiveToday,
            TotalEmployees: totalEmployeesTask.Result,
            AttendanceToday: attendanceTodayTask.Result,
            PendingLeaveRequests: pendingLeaveRequestsTask.Result,
            PendingOvertimeRequests: pendingOvertimeRequestsTask.Result,
            OpenJobPostings: openJobPostingsTask.Result,
            GeneratedAtUtc: DateTimeOffset.UtcNow);
    }
}

