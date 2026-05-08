using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeEmergencyContactFormVm
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    [StringLength(100)]
    public string? Relationship { get; set; }

    [Required, StringLength(50)]
    public string Phone { get; set; } = "";

    [StringLength(500)]
    public string? Address { get; set; }

    public bool IsPrimary { get; set; } = true;
}

