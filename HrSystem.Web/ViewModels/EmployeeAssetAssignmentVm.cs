using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeAssetAssignmentVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    public List<SelectListItem> Employees { get; set; } = [];

    [Required, StringLength(200)]
    public string AssetName { get; set; } = "";

    [StringLength(100)]
    public string? AssetTag { get; set; }

    [StringLength(100)]
    public string? SerialNumber { get; set; }

    [StringLength(200)]
    public string? AssignedBy { get; set; }

    [StringLength(500)]
    public string? ConditionOnAssign { get; set; }

    public AssetAssignmentStatus Status { get; set; } = AssetAssignmentStatus.Assigned;

    [StringLength(200)]
    public string? ReturnedTo { get; set; }

    [StringLength(500)]
    public string? ConditionOnReturn { get; set; }
}

