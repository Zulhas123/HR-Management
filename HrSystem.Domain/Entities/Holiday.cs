using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class Holiday : BaseEntity
{
    public DateOnly Date { get; set; }
    public required string Name { get; set; }
    public bool IsOptional { get; set; }
}

