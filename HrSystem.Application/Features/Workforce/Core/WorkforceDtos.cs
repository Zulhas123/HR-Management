namespace HrSystem.Application.Workforce;

public sealed record WorkforceEmployeeProductivityDto(
    int EmployeeId,
    string EmployeeCode,
    string FullName,
    int DepartmentId,
    string DepartmentName,
    int TasksAssigned,
    int TasksCompleted,
    int WorkLogDays,
    int WorkFromHomeDays,
    int MinutesLogged);

public sealed record WorkforceTeamPerformanceDto(
    int DepartmentId,
    string DepartmentName,
    int EmployeesCount,
    int TasksAssigned,
    int TasksCompleted,
    int WorkLogDays,
    int WorkFromHomeDays,
    int MinutesLogged);

public sealed record WorkforceProductivityReportDto(
    DateOnly FromInclusive,
    DateOnly ToInclusive,
    int? DepartmentId,
    IReadOnlyList<WorkforceEmployeeProductivityDto> Employees,
    IReadOnlyList<WorkforceTeamPerformanceDto> Teams);
