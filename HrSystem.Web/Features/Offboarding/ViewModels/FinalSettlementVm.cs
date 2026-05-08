using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class FinalSettlementVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeOffboardingId { get; set; }

    [Range(0, 100000000)]
    public decimal TotalPayable { get; set; }

    [Range(0, 100000000)]
    public decimal TotalDeductions { get; set; }

    public decimal NetPayable { get; set; }

    [StringLength(200)]
    public string? PreparedBy { get; set; }

    [StringLength(4000)]
    public string? Notes { get; set; }
}
