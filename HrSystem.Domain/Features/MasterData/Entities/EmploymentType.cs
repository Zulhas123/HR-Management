using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmploymentType : BaseEntity
{
    public required string Name { get; set; }
}
