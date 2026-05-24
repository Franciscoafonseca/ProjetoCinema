using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IRecomendacaoService
{
    Task<List<FilmeRecomendadoDTO>> ObterRecomendacoesAsync(int utilizadorId, int quantidade = 12);
}
