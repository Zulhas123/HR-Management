using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum ApplicationStage
{
    Applied = 0,
    Screening = 1,
    Interview = 2,
    Offered = 3,
    Hired = 4,
    Rejected = 5
}

public sealed class JobApplication : BaseEntity
{
    public int JobPostingId { get; set; }
    public JobPosting? JobPosting { get; set; }

    public int CandidateId { get; set; }
    public Candidate? Candidate { get; set; }

    public ApplicationStage Stage { get; set; } = ApplicationStage.Applied;
    public DateTime AppliedAtUtc { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
