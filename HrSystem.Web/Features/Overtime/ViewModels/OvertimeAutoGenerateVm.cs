using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class OvertimeAutoGenerateVm
{
    [Required]
    public DateTime FromInclusive { get; set; } = DateTime.Today.AddDays(-6);

    [Required]
    public DateTime ToInclusive { get; set; } = DateTime.Today;

    public bool CreateIfMissing { get; set; } = true;
}
