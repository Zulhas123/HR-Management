using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class DailyWorkLog : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly Date { get; set; }

    public int MinutesWorked { get; set; }

    public bool IsWorkFromHome { get; set; }

    public int? EmployeeTaskId { get; set; }
    public EmployeeTask? EmployeeTask { get; set; }

    public string? Summary { get; set; }
}
