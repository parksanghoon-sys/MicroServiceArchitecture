namespace Application.Core.Services;

public interface ICurrentUserService
{
    string? GetCurrentUser();
    int? GetCurrentUserId();
    bool IsAuthenticated();
}
