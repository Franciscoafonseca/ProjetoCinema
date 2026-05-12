using System.Security.Claims;

namespace OnlineCinemaFestival.Api.Services;

public class UtilizadorAtualService : IUtilizadorAtualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UtilizadorAtualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int ObterUtilizadorId()
    {
        var utilizador = _httpContextAccessor.HttpContext?.User;

        var valor = utilizador?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(valor, out var utilizadorId))
            throw new UnauthorizedAccessException("Token inválido.");

        return utilizadorId;
    }
}
