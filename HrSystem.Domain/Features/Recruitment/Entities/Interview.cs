using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum InterviewResult
{
    Pending = 0,
    Pass = 1,
    Fail = 2
}

public sealed class Interview : BaseEntity
{
    public int JobApplicationId { get; set; }
    public JobApplication? JobApplication { get; set; }

    public DateTime ScheduledAtUtc { get; set; }
    public string? Interviewer { get; set; }
    public string? Mode { get; set; } // Onsite / Online / Phone
    public string? Feedback { get; set; }
    public InterviewResult Result { get; set; } = InterviewResult.Pending;
}
