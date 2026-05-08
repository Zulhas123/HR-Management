using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class Shift : BaseEntity
{
    public required string Name { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsOvernight { get; set; }
}

