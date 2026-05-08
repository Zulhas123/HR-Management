using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class BloodGroup : BaseEntity
{
    public required string Name { get; set; } // e.g. A+, O-, AB+
}

