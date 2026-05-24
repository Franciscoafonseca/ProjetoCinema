using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaBilheteSessao : IPoliticaAcesso
{
    public TipoAcesso TipoSuportado => TipoAcesso.BilheteSessao;

    public int OrdemPreferencia => 0;

    public bool PermiteVisualizacao(
        AcessoUtilizador acesso,
        ContextoVisualizacao contexto,
        DateTime agora
    )
    {
        if (acesso.TipoAcesso != TipoSuportado || !ValidadeAcessoUtilizador.EstaAtivo(acesso, agora))
            return false;

        return contexto.SessaoId.HasValue
            && acesso.SessaoId == contexto.SessaoId.Value
            && contexto.InicioSessao.HasValue
            && contexto.FimSessao.HasValue
            && agora >= contexto.InicioSessao.Value
            && agora <= contexto.FimSessao.Value;
    }
}

