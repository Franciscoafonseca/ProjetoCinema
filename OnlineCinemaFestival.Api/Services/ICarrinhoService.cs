using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICarrinhoService
{
    IReadOnlyList<CompraItemDto> ObterItens(string utilizadorId);
    void AdicionarItem(string utilizadorId, CompraItemDto item);
    bool RemoverItem(string utilizadorId, int filmeId);
    void Limpar(string utilizadorId);
}
