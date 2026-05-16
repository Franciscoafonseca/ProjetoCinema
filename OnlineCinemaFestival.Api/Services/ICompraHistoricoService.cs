using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraHistoricoService
{
    Task RegistarAsync(
        string utilizadorId,
        decimal total,
        int pontosGanhos,
        List<CompraHistoricoItemDto> itens);
}
