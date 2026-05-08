using HrSystem.Application.Auth;

namespace HrSystem.Application.Abstractions;

public interface IAuthService
{
    Task<AuthResultDto?> AuthenticateAsync(
        string username,
        string password,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);
}
