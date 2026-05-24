using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaPasseCompleto : IPoliticaAcesso
{
    public TipoAcesso TipoSuportado => TipoAcesso.PasseCompleto;

    public int OrdemPreferencia => 3;

    public bool PermiteVisualizacao(
        AcessoUtilizador acesso,
        ContextoVisualizacao contexto,
        DateTime agora
    )
    {
        if (
            acesso.TipoAcesso != TipoSuportado
            || acesso.FestivalId is null
            || !ValidadeAcessoUtilizador.EstaAtivo(acesso, agora)
        )
            return false;

        if (contexto.SessaoId.HasValue)
            return acesso.FestivalId == contexto.FestivalId;

        if (!contexto.FilmeId.HasValue)
            return false;

        if (contexto.FestivalId.HasValue && acesso.FestivalId != contexto.FestivalId.Value)
            return false;

        return contexto.FestivalIdsDoFilme.Contains(acesso.FestivalId.Value);
    }
}

