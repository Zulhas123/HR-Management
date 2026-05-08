using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class Shift : BaseEntity
{
    public required string Name { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsOvernight { get; set; }

    // Flexible office hours (optional)
    public bool IsFlexibleHours { get; set; }
    public TimeOnly? FlexInStartTime { get; set; }
    public TimeOnly? FlexInEndTime { get; set; }
    public int GraceMinutes { get; set; }
    public int? RequiredWorkMinutes { get; set; }
}
