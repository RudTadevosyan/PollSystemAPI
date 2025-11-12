using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PollManagement.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(claim, out var userId)) //failed to take the userId from the jwt
            throw new UnauthorizedAccessException("Invalid userId");
        
        return userId;
    }
}