using System.Security.Claims;
using ReadingTracker.Api.Domain.Exceptions;

namespace ReadingTracker.Api.Controllers;

public static class ClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return Guid.TryParse(value, out var id)
            ? id
            : throw new UnauthorizedAppException("Token inválido.");
    }
}