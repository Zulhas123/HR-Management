using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class Designation : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

