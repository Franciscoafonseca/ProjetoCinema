using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public interface IPoliticaAcesso
{
    TipoAcesso TipoSuportado { get; }

    int OrdemPreferencia { get; }

    /// <summary>
    /// Indica se o acesso permite a visualização do conteúdo descrito pelo contexto.
    /// </summary>
    bool PermiteVisualizacao(
        AcessoUtilizador acesso,
        ContextoVisualizacao contexto,
        DateTime agora
    );

    /// <summary>
    /// Indica se o acesso está relacionado com o contexto de visualização — mesmo que
    /// expirado, inativo ou fora do horário — e deve, por isso, contribuir para a
    /// mensagem de negação apresentada ao utilizador.
    /// </summary>
    bool RelacionaComContexto(AcessoUtilizador acesso, ContextoVisualizacao contexto);

    /// <summary>
    /// Devolve a mensagem de negação mais adequada dado o conjunto de acessos
    /// relacionados com o contexto (todos do mesmo tipo gerido por esta política).
    /// </summary>
    string ObterMensagemNegacao(
        IReadOnlyList<AcessoUtilizador> acessosRelacionados,
        ContextoVisualizacao contexto,
        DateTime agora
    );
}
