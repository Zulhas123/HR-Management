using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class SimpleNameVm
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";
}

