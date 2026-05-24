using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoVisualizacaoService
{
    Task<AcessoUtilizador?> ObterAcessoValidoParaFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId
    );

    Task<AcessoUtilizador?> ObterAcessoValidoParaSessaoAsync(int utilizadorId, Sessao sessao);

    Task<ResultadoAcessoVisualizacao> ObterResultadoParaFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId
    );

    Task<ResultadoAcessoVisualizacao> ObterResultadoParaSessaoAsync(
        int utilizadorId,
        Sessao sessao
    );

    Task<bool> PodeVisualizarFilmeAsync(int utilizadorId, int filmeId, int? festivalId);

    Task<bool> PodeVisualizarSessaoAsync(int utilizadorId, Sessao sessao);
}

