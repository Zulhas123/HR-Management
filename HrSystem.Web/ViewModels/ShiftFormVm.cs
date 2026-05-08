using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class ShiftFormVm
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required]
    public TimeOnly StartTime { get; set; } = new(9, 0);

    [Required]
    public TimeOnly EndTime { get; set; } = new(18, 0);

    public bool IsOvernight { get; set; }
}

