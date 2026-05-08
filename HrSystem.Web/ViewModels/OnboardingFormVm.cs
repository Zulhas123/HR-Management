using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class OnboardingFormVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    public List<SelectListItem> Employees { get; set; } = [];

    public OnboardingStatus Status { get; set; } = OnboardingStatus.Draft;
}

