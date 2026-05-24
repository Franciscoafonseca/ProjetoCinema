using OnlineCinemaFestival.Api.Domain;

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

    public bool RelacionaComContexto(AcessoUtilizador acesso, ContextoVisualizacao contexto)
        => acesso.FestivalId.HasValue
           && (
               acesso.FestivalId == contexto.FestivalId
               || contexto.FestivalIdsDoFilme.Contains(acesso.FestivalId.Value)
           );

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
            return "O passe expirou.";

        if (acessosRelacionados.All(a => a.InicioValidade > agora))
            return "O passe ainda nao comecou.";

        return "Sem acesso valido para este conteudo.";
    }
}
