using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class WeekendConfiguration : BaseEntity
{
    public bool Sunday { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; } = true;
    public bool Saturday { get; set; } = true;
}

