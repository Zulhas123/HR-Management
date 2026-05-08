namespace HrSystem.Application.Auth;

public sealed record AuthResultDto(
    int UserId,
    string Username,
    string DisplayName,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
