using OnlineCinemaFestival.Api.Services.PoliticasAcesso;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public sealed class AcessoVisualizacaoService : IAcessoVisualizacaoService
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly IVisualizacaoRepository _visualizacaoRepository;
    private readonly IPoliticaAcessoResolver _politicaAcessoResolver;
    private readonly TimeProvider _timeProvider;

    public AcessoVisualizacaoService(
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        IVisualizacaoRepository visualizacaoRepository,
        IPoliticaAcessoResolver politicaAcessoResolver,
        TimeProvider timeProvider
    )
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _visualizacaoRepository = visualizacaoRepository;
        _politicaAcessoResolver = politicaAcessoResolver;
        _timeProvider = timeProvider;
    }

    public async Task<AcessoUtilizador?> ObterAcessoValidoParaFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId
    )
    {
        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        var festivalIdsDoFilme = await _visualizacaoRepository.ObterFestivalIdsDoFilmeAsync(filme.Id);
        var contexto = new ContextoVisualizacao(
            filme.Id,
            null,
            festivalId,
            null,
            null,
            festivalIdsDoFilme
        );

        return (await ObterResultadoAsync(utilizadorId, contexto, agora)).Acesso;
    }

    public async Task<AcessoUtilizador?> ObterAcessoValidoParaSessaoAsync(
        int utilizadorId,
        Sessao sessao
    )
    {
        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        var contexto = new ContextoVisualizacao(
            sessao.FilmeId,
            sessao.Id,
            sessao.FestivalId,
            sessao.Inicio,
            sessao.Fim,
            new HashSet<int> { sessao.FestivalId }
        );

        return (await ObterResultadoAsync(utilizadorId, contexto, agora)).Acesso;
    }

    public async Task<ResultadoAcessoVisualizacao> ObterResultadoParaFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId
    )
    {
        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        var festivalIdsDoFilme = await _visualizacaoRepository.ObterFestivalIdsDoFilmeAsync(filme.Id);
        var contexto = new ContextoVisualizacao(
            filme.Id,
            null,
            festivalId,
            null,
            null,
            festivalIdsDoFilme
        );

        return await ObterResultadoAsync(utilizadorId, contexto, agora);
    }

    public async Task<ResultadoAcessoVisualizacao> ObterResultadoParaSessaoAsync(
        int utilizadorId,
        Sessao sessao
    )
    {
        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        var contexto = new ContextoVisualizacao(
            sessao.FilmeId,
            sessao.Id,
            sessao.FestivalId,
            sessao.Inicio,
            sessao.Fim,
            new HashSet<int> { sessao.FestivalId }
        );

        return await ObterResultadoAsync(utilizadorId, contexto, agora);
    }

    public async Task<bool> PodeVisualizarFilmeAsync(int utilizadorId, int filmeId, int? festivalId)
    {
        var filme = await _visualizacaoRepository.ObterFilmePorIdAsync(filmeId);

        if (filme == null)
            return false;

        return await ObterAcessoValidoParaFilmeAsync(utilizadorId, filme, festivalId) != null;
    }

    public async Task<bool> PodeVisualizarSessaoAsync(int utilizadorId, Sessao sessao)
    {
        return await ObterAcessoValidoParaSessaoAsync(utilizadorId, sessao) != null;
    }

    private async Task<ResultadoAcessoVisualizacao> ObterResultadoAsync(
        int utilizadorId,
        ContextoVisualizacao contexto,
        DateTime agora
    )
    {
        var acessos = (
            await _acessoUtilizadorRepository.ObterPorUtilizadorIdAsync(utilizadorId)
        ).ToList();

        var acessoValido = acessos
            .Select(acesso => new
            {
                Acesso = acesso,
                Politica = _politicaAcessoResolver.Resolver(acesso.TipoAcesso),
            })
            .OrderBy(item => item.Politica.OrdemPreferencia)
            .FirstOrDefault(item =>
                item.Politica.PermiteVisualizacao(item.Acesso, contexto, agora)
            )
            ?.Acesso;

        if (acessoValido != null)
            return ResultadoAcessoVisualizacao.Autorizado(acessoValido);

        var relacionados = acessos
            .Where(acesso => AcessoRelacionaComContexto(acesso, contexto))
            .OrderByDescending(a => a.FimValidade)
            .ToList();

        return ResultadoAcessoVisualizacao.Negado(
            ObterMensagemNegacao(relacionados, contexto, agora)
        );
    }

    private static bool AcessoRelacionaComContexto(
        AcessoUtilizador acesso,
        ContextoVisualizacao contexto
    )
    {
        return acesso.TipoAcesso switch
        {
            TipoAcesso.BilheteSessao =>
                contexto.SessaoId.HasValue && acesso.SessaoId == contexto.SessaoId.Value,
            TipoAcesso.AluguerDigital =>
                contexto.FilmeId.HasValue && acesso.FilmeId == contexto.FilmeId.Value,
            TipoAcesso.PasseDiario =>
                contexto.SessaoId.HasValue
                && contexto.InicioSessao.HasValue
                && acesso.FestivalId == contexto.FestivalId
                && contexto.InicioSessao.Value >= acesso.InicioValidade
                && contexto.InicioSessao.Value < acesso.FimValidade,
            TipoAcesso.PasseCompleto =>
                acesso.FestivalId.HasValue
                && (
                    acesso.FestivalId == contexto.FestivalId
                    || contexto.FestivalIdsDoFilme.Contains(acesso.FestivalId.Value)
                ),
            _ => false,
        };
    }

    private static string ObterMensagemNegacao(
        IReadOnlyCollection<AcessoUtilizador> acessosRelacionados,
        ContextoVisualizacao contexto,
        DateTime agora
    )
    {
        if (acessosRelacionados.Count == 0)
            return "Sem acesso valido para este conteudo.";

        if (acessosRelacionados.Any(a => !a.Ativo))
            return "O acesso existe, mas esta inativo.";

        if (contexto.SessaoId.HasValue && contexto.InicioSessao.HasValue && agora < contexto.InicioSessao.Value)
            return "Esta sessao ainda nao comecou.";

        if (contexto.SessaoId.HasValue && contexto.FimSessao.HasValue && agora > contexto.FimSessao.Value)
            return "Esta sessao ja terminou.";

        if (acessosRelacionados.All(a => a.FimValidade < agora))
            return "O acesso expirou.";

        if (acessosRelacionados.All(a => a.InicioValidade > agora))
            return "O acesso ainda nao comecou.";

        return "Sem acesso valido para este conteudo.";
    }
}
