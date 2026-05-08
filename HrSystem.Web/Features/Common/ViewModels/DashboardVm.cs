using HrSystem.Application.Features.Common.Models;

namespace HrSystem.Web.ViewModels;

public sealed class DashboardVm
{
    public required DashboardSummary Summary { get; init; }
    public OvertimePolicyVm? OvertimePolicy { get; init; }
    public WeekendConfigVm? WeekendConfig { get; init; }
    public IReadOnlyList<ImportantPolicyVm> ImportantPolicies { get; init; } = [];
    public int RefreshSeconds { get; init; } = 30;
}

public sealed record OvertimePolicyVm(DateOnly EffectiveFrom, decimal NormalMultiplier, decimal HolidayMultiplier, int ApprovalLevelsRequired);

public sealed record WeekendConfigVm(bool Friday, bool Saturday, bool Sunday, bool Monday, bool Tuesday, bool Wednesday, bool Thursday);

public sealed record ImportantPolicyVm(string Title, string Body, string? Url);

