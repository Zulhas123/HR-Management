using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class FinalSettlement : BaseEntity
{
    public int EmployeeOffboardingId { get; set; }
    public EmployeeOffboarding? EmployeeOffboarding { get; set; }

    public decimal TotalPayable { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPayable { get; set; }

    public DateTimeOffset PreparedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? PreparedBy { get; set; }
    public string? Notes { get; set; }
}
