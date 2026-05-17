using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface ICinemaFacade
{
    Task ComprarItens(int utilizadorId, List<CompraItemDto> itens);
}
