using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class LoginVm
{
    [Required, StringLength(100)]
    public string Username { get; set; } = "";

    [Required, DataType(DataType.Password), StringLength(200)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

