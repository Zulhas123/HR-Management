using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class Religion : BaseEntity
{
    public required string Name { get; set; }
}
