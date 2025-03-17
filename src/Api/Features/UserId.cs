using System.Security.Claims;

namespace Wrpg;

public static class UserId
{
    public const int MaxLength = 50;

    internal const string ClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

    public static string ResolveFrom(ClaimsPrincipal user)
    {
        var userId = user.Claims.FirstOrDefault(x => x.Type == ClaimType)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            throw new ApplicationException("User is missing name identifier");

        return userId;
    }
}