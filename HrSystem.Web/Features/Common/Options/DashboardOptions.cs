namespace HrSystem.Web.Options;

public sealed class DashboardOptions
{
    public int RefreshSeconds { get; set; } = 30;
    public List<ImportantPolicyOptions> ImportantPolicies { get; set; } = [];
}

public sealed class ImportantPolicyOptions
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string? Url { get; set; }
}

