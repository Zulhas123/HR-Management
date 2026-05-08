using HrSystem.Application.Abstractions;
using HrSystem.Application.Workforce;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class WorkforceReportingService(
    IEmployeeRepository employees,
    IEmployeeTaskRepository tasks,
    IDailyWorkLogRepository workLogs) : IWorkforceReportingService
{
    public async Task<WorkforceProductivityReportDto> GetProductivityReportAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        int? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        if (toInclusive < fromInclusive)
        {
            throw new ArgumentException("toInclusive must be >= fromInclusive");
        }

        var employeeList = await employees.ListAsync(cancellationToken);
        if (departmentId.HasValue)
        {
            employeeList = employeeList.Where(e => e.DepartmentId == departmentId.Value).ToList();
        }

        var employeeById = employeeList.ToDictionary(e => e.Id);
        var departmentNameById = employeeList
            .Select(e => e.Department)
            .Where(d => d is not null)
            .GroupBy(d => d!.Id)
            .ToDictionary(g => g.Key, g => g.First()!.Name);

        var taskList = await tasks.ListByAssignedDateRangeAsync(fromInclusive, toInclusive, cancellationToken);
        var filteredTasks = taskList.Where(t => employeeById.ContainsKey(t.EmployeeId)).ToList();

        var workLogList = await workLogs.ListByDateRangeAsync(fromInclusive, toInclusive, cancellationToken);
        var filteredLogs = workLogList.Where(l => employeeById.ContainsKey(l.EmployeeId)).ToList();

        var employeeSummaries = employeeList.ToDictionary(
            e => e.Id,
            e => new MutableEmployee(
                e.Id,
                e.EmployeeCode,
                $"{e.FirstName} {e.LastName}".Trim(),
                e.DepartmentId,
                e.Department?.Name ?? departmentNameById.GetValueOrDefault(e.DepartmentId, $"Dept-{e.DepartmentId}")));

        foreach (var t in filteredTasks)
        {
            var s = employeeSummaries[t.EmployeeId];
            s.TasksAssigned += 1;
            if (t.Status == Domain.Entities.EmployeeTaskStatus.Completed)
            {
                s.TasksCompleted += 1;
            }
        }

        foreach (var log in filteredLogs)
        {
            var s = employeeSummaries[log.EmployeeId];
            s.WorkLogDays += 1;
            if (log.IsWorkFromHome)
            {
                s.WorkFromHomeDays += 1;
            }
            s.MinutesLogged += Math.Max(0, log.MinutesWorked);
        }

        var employeesDto = employeeSummaries.Values
            .OrderBy(x => x.DepartmentName)
            .ThenBy(x => x.EmployeeCode)
            .Select(x => x.ToDto())
            .ToList();

        var teams = employeesDto
            .GroupBy(e => new { e.DepartmentId, e.DepartmentName })
            .Select(g => new WorkforceTeamPerformanceDto(
                g.Key.DepartmentId,
                g.Key.DepartmentName,
                EmployeesCount: g.Count(),
                TasksAssigned: g.Sum(x => x.TasksAssigned),
                TasksCompleted: g.Sum(x => x.TasksCompleted),
                WorkLogDays: g.Sum(x => x.WorkLogDays),
                WorkFromHomeDays: g.Sum(x => x.WorkFromHomeDays),
                MinutesLogged: g.Sum(x => x.MinutesLogged)))
            .OrderBy(x => x.DepartmentName)
            .ToList();

        return new WorkforceProductivityReportDto(fromInclusive, toInclusive, departmentId, employeesDto, teams);
    }

    private sealed class MutableEmployee(
        int employeeId,
        string employeeCode,
        string fullName,
        int departmentId,
        string departmentName)
    {
        public int EmployeeId { get; } = employeeId;
        public string EmployeeCode { get; } = employeeCode;
        public string FullName { get; } = fullName;
        public int DepartmentId { get; } = departmentId;
        public string DepartmentName { get; } = departmentName;

        public int TasksAssigned { get; set; }
        public int TasksCompleted { get; set; }
        public int WorkLogDays { get; set; }
        public int WorkFromHomeDays { get; set; }
        public int MinutesLogged { get; set; }

        public WorkforceEmployeeProductivityDto ToDto() =>
            new(
                EmployeeId,
                EmployeeCode,
                FullName,
                DepartmentId,
                DepartmentName,
                TasksAssigned,
                TasksCompleted,
                WorkLogDays,
                WorkFromHomeDays,
                MinutesLogged);
    }
}
