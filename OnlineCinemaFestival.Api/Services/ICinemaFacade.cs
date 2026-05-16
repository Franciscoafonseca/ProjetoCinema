using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICinemaFacade
{
    Task ComprarItens(string utilizadorId, List<CompraItemDto> itens);
}
