using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class LeaveDecisionVm
{
    public int Id { get; set; }

    [StringLength(500)]
    public string? DecisionNote { get; set; }
}

