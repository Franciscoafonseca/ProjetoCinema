using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public interface IEstrategiaValidacaoAcesso
{
    TipoAcesso Tipo { get; }

    Task<AcessoUtilizador?> ValidarFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId,
        DateTime agora
    );

    Task<AcessoUtilizador?> ValidarSessaoAsync(
        int utilizadorId,
        Sessao sessao,
        DateTime agora
    );
}
