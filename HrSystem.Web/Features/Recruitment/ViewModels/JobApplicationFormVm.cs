using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class JobApplicationFormVm
{
    public int Id { get; set; }

    [Required]
    public int JobPostingId { get; set; }
    public List<SelectListItem> JobPostings { get; set; } = [];

    [Required]
    public int CandidateId { get; set; }
    public List<SelectListItem> Candidates { get; set; } = [];

    [Required]
    public ApplicationStage Stage { get; set; } = ApplicationStage.Applied;

    [StringLength(2000)]
    public string? Notes { get; set; }
}
