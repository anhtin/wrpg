using System.Security.Claims;

namespace Wrpg;

public static class UserId
{
    public static string ResolveFrom(ClaimsPrincipal user)
    {
        var userId = user.Claims
            .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
            ?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            throw new ApplicationException("User is missing name identifier");

        return userId;
    }
}