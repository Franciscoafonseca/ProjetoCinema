using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IValidacaoAcessoService
{
    Task<bool> PodeVisualizarFilmeAsync(int utilizadorId, int filmeId, int? festivalId);

    Task<bool> PodeVisualizarSessaoAsync(int utilizadorId, Sessao sessao);
}
