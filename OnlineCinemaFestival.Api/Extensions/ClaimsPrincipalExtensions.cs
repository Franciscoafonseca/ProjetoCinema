using System.Security.Claims;

namespace OnlineCinemaFestival.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("id")?.Value;

        if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int id))
        {
            throw new UnauthorizedAccessException("ID do utilizador não encontrado no Token.");
        }

        return id;
    }
}
