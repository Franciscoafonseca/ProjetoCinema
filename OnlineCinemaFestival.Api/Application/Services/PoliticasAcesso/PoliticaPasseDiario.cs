using OnlineCinemaFestival.Api.Domain;

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

    public bool RelacionaComContexto(AcessoUtilizador acesso, ContextoVisualizacao contexto)
        => contexto.SessaoId.HasValue
           && contexto.InicioSessao.HasValue
           && acesso.FestivalId == contexto.FestivalId
           && contexto.InicioSessao.Value >= acesso.InicioValidade
           && contexto.InicioSessao.Value < acesso.FimValidade;

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

        if (acessosRelacionados.All(a => a.FimValidade < agora))
            return "O passe diario expirou.";

        if (acessosRelacionados.All(a => a.InicioValidade > agora))
            return "O passe diario ainda nao comecou.";

        return "Sem acesso valido para este conteudo.";
    }
}
