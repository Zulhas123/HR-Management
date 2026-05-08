namespace HrSystem.Web.ViewModels;

public sealed class WeekendConfigurationVm
{
    public int Id { get; set; }

    public bool Sunday { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; } = true;
    public bool Saturday { get; set; } = true;
}

