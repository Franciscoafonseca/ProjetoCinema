using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaPasseDiario : IPoliticaAcesso
{
    public TipoAcesso TipoSuportado => TipoAcesso.PasseDiario;

    public int OrdemPreferencia => 2;

    public bool PermiteVisualizacao(
        AcessoUtilizador acesso,
        ContextoVisualizacao contexto,
        DateTime agora
    )
    {
        if (acesso.TipoAcesso != TipoSuportado || !ValidadeAcessoUtilizador.EstaAtivo(acesso, agora))
            return false;

        return contexto.SessaoId.HasValue
            && contexto.InicioSessao.HasValue
            && acesso.FestivalId == contexto.FestivalId
            && contexto.InicioSessao.Value >= acesso.InicioValidade
            && contexto.InicioSessao.Value < acesso.FimValidade;
    }
}

