using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class HolidayFormVm
{
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    public bool IsOptional { get; set; }
}

