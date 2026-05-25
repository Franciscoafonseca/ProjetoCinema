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
        var candidatos = (
            await _acessoUtilizadorRepository.ObterPorUtilizadorIdAsync(utilizadorId)
        )
            .Select(acesso => new
            {
                Acesso = acesso,
                Politica = _politicaAcessoResolver.Resolver(acesso.TipoAcesso),
            })
            .OrderBy(item => item.Politica.OrdemPreferencia)
            .ToList();

        // Verificar se algum acesso permite visualização
        var acessoValido = candidatos
            .FirstOrDefault(item =>
                item.Politica.PermiteVisualizacao(item.Acesso, contexto, agora)
            )
            ?.Acesso;

        if (acessoValido != null)
            return ResultadoAcessoVisualizacao.Autorizado(acessoValido);

        // Acessos que se relacionam com o contexto (podem estar expirados, inativos, etc.)
        var relacionados = candidatos
            .Where(item => item.Politica.RelacionaComContexto(item.Acesso, contexto))
            .ToList();

        if (relacionados.Count == 0)
            return ResultadoAcessoVisualizacao.Negado("Sem acesso valido para este conteudo.");

        // Delegar a mensagem na política mais específica (menor OrdemPreferencia)
        var melhorPolitica = relacionados
            .Select(r => r.Politica)
            .OrderBy(p => p.OrdemPreferencia)
            .First();

        var acessosDaPolitica = relacionados
            .Where(r => r.Politica.TipoSuportado == melhorPolitica.TipoSuportado)
            .Select(r => r.Acesso)
            .ToList();

        return ResultadoAcessoVisualizacao.Negado(
            melhorPolitica.ObterMensagemNegacao(acessosDaPolitica, contexto, agora)
        );
    }
}
