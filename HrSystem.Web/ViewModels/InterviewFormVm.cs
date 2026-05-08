using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class InterviewFormVm
{
    public int Id { get; set; }

    [Required]
    public int JobApplicationId { get; set; }
    public List<SelectListItem> Applications { get; set; } = [];

    [Required]
    public DateTime ScheduledAtUtc { get; set; } = DateTime.UtcNow.AddDays(1);

    [StringLength(200)]
    public string? Interviewer { get; set; }

    [StringLength(50)]
    public string? Mode { get; set; }

    [StringLength(2000)]
    public string? Feedback { get; set; }

    public InterviewResult Result { get; set; } = InterviewResult.Pending;
}

