using OnlineCinemaFestival.Api.Domain;

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

    public bool RelacionaComContexto(AcessoUtilizador acesso, ContextoVisualizacao contexto)
        => contexto.SessaoId.HasValue && acesso.SessaoId == contexto.SessaoId.Value;

    public string ObterMensagemNegacao(
        IReadOnlyList<AcessoUtilizador> acessosRelacionados,
        ContextoVisualizacao contexto,
        DateTime agora
    )
    {
        if (acessosRelacionados.Count == 0)
            return "Sem acesso valido para este conteudo.";

        if (acessosRelacionados.Any(a => !a.Ativo))
            return "O acesso existe, mas esta inativo.";

        if (contexto.InicioSessao.HasValue && agora < contexto.InicioSessao.Value)
            return "Esta sessao ainda nao comecou.";

        if (contexto.FimSessao.HasValue && agora > contexto.FimSessao.Value)
            return "Esta sessao ja terminou.";

        if (acessosRelacionados.All(a => a.FimValidade < agora))
            return "O bilhete expirou.";

        return "Sem acesso valido para este conteudo.";
    }
}
