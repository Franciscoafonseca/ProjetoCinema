using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaAluguerDigital : IPoliticaAcesso
{
    public TipoAcesso TipoSuportado => TipoAcesso.AluguerDigital;

    public int OrdemPreferencia => 1;

    public bool PermiteVisualizacao(
        AcessoUtilizador acesso,
        ContextoVisualizacao contexto,
        DateTime agora
    )
    {
        if (acesso.TipoAcesso != TipoSuportado || !ValidadeAcessoUtilizador.EstaAtivo(acesso, agora))
            return false;

        return contexto.FilmeId.HasValue && acesso.FilmeId == contexto.FilmeId.Value;
    }
}

