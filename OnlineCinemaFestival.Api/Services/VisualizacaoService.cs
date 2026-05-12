using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class VisualizacaoService : IVisualizacaoService
{
    private readonly IVisualizacaoRepository _visualizacaoRepository;
    private readonly IValidacaoAcessoService _validacaoAcessoService;
    private readonly ITmdbService _tmdbService;

    public VisualizacaoService(
        IVisualizacaoRepository visualizacaoRepository,
        IValidacaoAcessoService validacaoAcessoService,
        ITmdbService tmdbService
    )
    {
        _visualizacaoRepository = visualizacaoRepository;
        _validacaoAcessoService = validacaoAcessoService;
        _tmdbService = tmdbService;
    }

    public async Task<VisualizacaoReadDto> ObterVisualizacaoFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId
    )
    {
        var filme = await _visualizacaoRepository.GetFilmeByIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme não encontrado.");

        var podeVisualizar = await _validacaoAcessoService.PodeVisualizarFilmeAsync(
            utilizadorId,
            filmeId,
            festivalId
        );

        if (!podeVisualizar)
            throw new UnauthorizedAccessException(
                "Não possui acesso válido para visualizar este filme."
            );

        var url = await ObterUrlVisualizacaoAsync(filme.TmdbId);

        return new VisualizacaoReadDto
        {
            TipoConteudo = "Filme",
            FilmeId = filme.Id,
            SessaoId = null,
            TemChatAoVivo = false,
            Mensagem = "Acesso autorizado ao filme.",
            Conteudos = new List<ConteudoVisualizacaoDto>
            {
                new()
                {
                    FilmeId = filme.Id,
                    Titulo = filme.Titulo,
                    Ordem = 1,
                    UrlVisualizacao = url,
                },
            },
        };
    }

    public async Task<VisualizacaoReadDto> ObterVisualizacaoSessaoAsync(
        int utilizadorId,
        int sessaoId
    )
    {
        var sessao = await _visualizacaoRepository.GetSessaoByIdAsync(sessaoId);

        if (sessao == null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        var podeVisualizar = await _validacaoAcessoService.PodeVisualizarSessaoAsync(
            utilizadorId,
            sessao
        );

        if (!podeVisualizar)
            throw new UnauthorizedAccessException(
                "Não possui acesso válido para visualizar esta sessão."
            );

        var conteudos = new List<ConteudoVisualizacaoDto>();

        foreach (var sessaoFilme in sessao.FilmesDaSessao.OrderBy(sf => sf.Ordem))
        {
            var filme = sessaoFilme.Filme;

            if (filme == null)
                continue;

            var url = await ObterUrlVisualizacaoAsync(filme.TmdbId);

            conteudos.Add(
                new ConteudoVisualizacaoDto
                {
                    FilmeId = filme.Id,
                    Titulo = filme.Titulo,
                    Ordem = sessaoFilme.Ordem,
                    UrlVisualizacao = url,
                }
            );
        }

        return new VisualizacaoReadDto
        {
            TipoConteudo = "Sessao",
            FilmeId = null,
            SessaoId = sessao.Id,
            TemChatAoVivo = sessao.TemChatAoVivo,
            Mensagem = "Acesso autorizado à sessão.",
            Conteudos = conteudos,
        };
    }

    private async Task<string> ObterUrlVisualizacaoAsync(int tmdbId)
    {
        var trailerUrl = await _tmdbService.GetTrailerUrlAsync(tmdbId);

        if (!string.IsNullOrWhiteSpace(trailerUrl))
            return trailerUrl;

        return "https://www.youtube.com/embed/dQw4w9WgXcQ";
    }
}
