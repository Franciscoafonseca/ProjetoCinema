using OnlineCinemaFestival.Api.Domain;

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

    public bool RelacionaComContexto(AcessoUtilizador acesso, ContextoVisualizacao contexto)
        => contexto.FilmeId.HasValue && acesso.FilmeId == contexto.FilmeId.Value;

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
            return "O aluguer expirou.";

        if (acessosRelacionados.All(a => a.InicioValidade > agora))
            return "O aluguer ainda nao comecou.";

        return "Sem acesso valido para este conteudo.";
    }
}
