using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HrSystem.Web.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class PermissionAttribute(string permission) : Attribute, IAsyncAuthorizationFilter
{
    public string Permission { get; } = permission;

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return Task.CompletedTask;
        }

        // Super Admin bypass
        if (user.IsInRole("Super Admin"))
        {
            return Task.CompletedTask;
        }

        var has = user.HasClaim(c => string.Equals(c.Type, "permission", StringComparison.OrdinalIgnoreCase) &&
                                     string.Equals(c.Value, Permission, StringComparison.OrdinalIgnoreCase));
        if (!has)
        {
            context.Result = new ForbidResult();
        }

        return Task.CompletedTask;
    }
}

