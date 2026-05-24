using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public interface IPoliticaAcesso
{
    TipoAcesso TipoSuportado { get; }

    int OrdemPreferencia { get; }

    bool PermiteVisualizacao(
        AcessoUtilizador acesso,
        ContextoVisualizacao contexto,
        DateTime agora
    );
}
