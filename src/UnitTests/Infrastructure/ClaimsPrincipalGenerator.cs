using System.Security.Claims;
using Wrpg;

namespace Infrastructure;

public static class ClaimsPrincipalGenerator
{
    public static ClaimsPrincipal Create(string? userId = null, string[]? permission = null)
    {
        var claims = new List<Claim>();
        if (userId is not null)
            claims.Add(new Claim(UserId.ClaimType, userId));
        if (permission is not null)
            claims.Add(new Claim("scopes", string.Join(' ', permission)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }
}