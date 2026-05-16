using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
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

    public async Task<VisualizacaoReadDTO> ObterVisualizacaoFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId
    )
    {
        var filme = await _visualizacaoRepository.ObterFilmePorIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        var acesso = await _validacaoAcessoService.ObterAcessoValidoParaFilmeAsync(
            utilizadorId,
            filme,
            festivalId
        );

        if (acesso == null)
            throw new UnauthorizedAccessException(
                "Nao possui acesso valido para visualizar este filme."
            );

        var url = await ObterUrlVisualizacaoAsync(filme);

        await RegistarVisualizacaoInternaAsync(
            utilizadorId,
            filme.Id,
            null,
            acesso.FestivalId ?? festivalId,
            "Filme",
            acesso.TipoAcesso,
            url
        );

        return new VisualizacaoReadDTO
        {
            TipoConteudo = "Filme",
            FilmeId = filme.Id,
            SessaoId = null,
            TemChatAoVivo = false,
            Mensagem = "Acesso autorizado ao filme.",
            Conteudos = new List<ConteudoVisualizacaoDTO>
            {
                new()
                {
                    FilmeId = filme.Id,
                    Titulo = filme.Titulo,
                    PosterUrl = filme.CapaUrl,
                    Ordem = 1,
                    UrlVisualizacao = url,
                },
            },
        };
    }

    public async Task<VisualizacaoReadDTO> ObterVisualizacaoSessaoAsync(
        int utilizadorId,
        int sessaoId
    )
    {
        var sessao = await _visualizacaoRepository.ObterSessaoPorIdAsync(sessaoId);

        if (sessao == null)
            throw new KeyNotFoundException("Sessao nao encontrada.");

        var acesso = await _validacaoAcessoService.ObterAcessoValidoParaSessaoAsync(
            utilizadorId,
            sessao
        );

        if (acesso == null)
            throw new UnauthorizedAccessException(
                "Nao possui acesso valido para visualizar esta sessao."
            );

        var conteudos = new List<ConteudoVisualizacaoDTO>();

        foreach (var sessaoFilme in sessao.FilmesDaSessao.OrderBy(sf => sf.Ordem))
        {
            var filme = sessaoFilme.Filme;

            if (filme == null)
                continue;

            var url = await ObterUrlVisualizacaoAsync(filme);

            conteudos.Add(
                new ConteudoVisualizacaoDTO
                {
                    FilmeId = filme.Id,
                    Titulo = filme.Titulo,
                    PosterUrl = filme.CapaUrl,
                    Ordem = sessaoFilme.Ordem,
                    UrlVisualizacao = url,
                }
            );
        }

        await RegistarVisualizacoesSessaoAsync(
            utilizadorId,
            sessao.Id,
            sessao.FestivalId,
            acesso.TipoAcesso,
            conteudos
        );

        return new VisualizacaoReadDTO
        {
            TipoConteudo = "Sessao",
            FilmeId = null,
            SessaoId = sessao.Id,
            TemChatAoVivo = sessao.TemChatAoVivo,
            Mensagem = "Acesso autorizado a sessao.",
            Conteudos = conteudos,
        };
    }

    public async Task<IEnumerable<VisualizacaoHistoricoReadDTO>> ObterHistoricoDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var visualizacoes = await _visualizacaoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        return visualizacoes.Select(MapToHistoricoDTO);
    }

    public async Task<VisualizacaoHistoricoReadDTO> RegistarVisualizacaoAsync(
        int utilizadorId,
        RegistarVisualizacaoDTO dto
    )
    {
        var filme = await _visualizacaoRepository.ObterFilmePorIdAsync(dto.FilmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        var tipoConteudo = "Filme";
        int? festivalId = dto.FestivalId;
        TipoAcesso? tipoAcessoUsado;

        if (dto.SessaoId.HasValue)
        {
            var sessao = await _visualizacaoRepository.ObterSessaoPorIdAsync(dto.SessaoId.Value);

            if (sessao == null)
                throw new KeyNotFoundException("Sessao nao encontrada.");

            if (!sessao.FilmesDaSessao.Any(sf => sf.FilmeId == dto.FilmeId))
                throw new InvalidOperationException("O filme nao pertence a esta sessao.");

            var acesso = await _validacaoAcessoService.ObterAcessoValidoParaSessaoAsync(
                utilizadorId,
                sessao
            );

            if (acesso == null)
                throw new UnauthorizedAccessException("Sem acesso valido para esta sessao.");

            tipoConteudo = "Sessao";
            festivalId = sessao.FestivalId;
            tipoAcessoUsado = acesso.TipoAcesso;
        }
        else
        {
            var acesso = await _validacaoAcessoService.ObterAcessoValidoParaFilmeAsync(
                utilizadorId,
                filme,
                dto.FestivalId
            );

            if (acesso == null)
                throw new UnauthorizedAccessException("Sem acesso valido para este filme.");

            festivalId = acesso.FestivalId ?? festivalId;
            tipoAcessoUsado = acesso.TipoAcesso;
        }

        var url = await ObterUrlVisualizacaoAsync(filme);

        var visualizacao = new Visualizacao
        {
            UtilizadorId = utilizadorId,
            FilmeId = filme.Id,
            SessaoId = dto.SessaoId,
            FestivalId = festivalId,
            TipoConteudo = tipoConteudo,
            TipoAcessoUsado = tipoAcessoUsado,
            UrlVisualizacao = url,
            VisualizadoEm = DateTime.UtcNow,
        };

        await _visualizacaoRepository.AddAsync(visualizacao);
        await _visualizacaoRepository.SaveChangesAsync();

        return new VisualizacaoHistoricoReadDTO
        {
            Id = visualizacao.Id,
            FilmeId = filme.Id,
            FilmeTitulo = filme.Titulo,
            FilmePosterUrl = filme.CapaUrl,
            SessaoId = visualizacao.SessaoId,
            FestivalId = visualizacao.FestivalId,
            TipoConteudo = visualizacao.TipoConteudo,
            TipoAcessoUsado = visualizacao.TipoAcessoUsado?.ToString() ?? string.Empty,
            UrlVisualizacao = ObterUrlHistorico(visualizacao),
            VisualizadoEm = visualizacao.VisualizadoEm,
        };
    }

    private async Task<string> ObterUrlVisualizacaoAsync(Filme filme)
    {
        if (!string.IsNullOrWhiteSpace(filme.VideoUrl))
            return filme.VideoUrl;

        if (!string.IsNullOrWhiteSpace(filme.TrailerUrl))
            return filme.TrailerUrl;

        if (!string.IsNullOrWhiteSpace(filme.ConteudoLocalPath))
            return filme.ConteudoLocalPath;

        var trailerUrl = await _tmdbService.ObterTrailerUrlAsync(filme.TmdbId);

        if (!string.IsNullOrWhiteSpace(trailerUrl))
            return trailerUrl;

        return "https://www.youtube.com/embed/dQw4w9WgXcQ";
    }

    private async Task RegistarVisualizacaoInternaAsync(
        int utilizadorId,
        int filmeId,
        int? sessaoId,
        int? festivalId,
        string tipoConteudo,
        TipoAcesso tipoAcessoUsado,
        string urlVisualizacao
    )
    {
        await _visualizacaoRepository.AddAsync(
            new Visualizacao
            {
                UtilizadorId = utilizadorId,
                FilmeId = filmeId,
                SessaoId = sessaoId,
                FestivalId = festivalId,
                TipoConteudo = tipoConteudo,
                TipoAcessoUsado = tipoAcessoUsado,
                UrlVisualizacao = urlVisualizacao,
                VisualizadoEm = DateTime.UtcNow,
            }
        );

        await _visualizacaoRepository.SaveChangesAsync();
    }

    private async Task RegistarVisualizacoesSessaoAsync(
        int utilizadorId,
        int sessaoId,
        int festivalId,
        TipoAcesso tipoAcessoUsado,
        IEnumerable<ConteudoVisualizacaoDTO> conteudos
    )
    {
        var agora = DateTime.UtcNow;
        var visualizacoes = conteudos.Select(conteudo => new Visualizacao
        {
            UtilizadorId = utilizadorId,
            FilmeId = conteudo.FilmeId,
            SessaoId = sessaoId,
            FestivalId = festivalId,
            TipoConteudo = "Sessao",
            TipoAcessoUsado = tipoAcessoUsado,
            UrlVisualizacao = conteudo.UrlVisualizacao,
            VisualizadoEm = agora,
        });

        await _visualizacaoRepository.AddRangeAsync(visualizacoes);
        await _visualizacaoRepository.SaveChangesAsync();
    }

    private static VisualizacaoHistoricoReadDTO MapToHistoricoDTO(Visualizacao visualizacao)
    {
        return new VisualizacaoHistoricoReadDTO
        {
            Id = visualizacao.Id,
            FilmeId = visualizacao.FilmeId,
            FilmeTitulo = visualizacao.Filme?.Titulo ?? string.Empty,
            FilmePosterUrl = visualizacao.Filme?.CapaUrl ?? string.Empty,
            SessaoId = visualizacao.SessaoId,
            SessaoInicio = visualizacao.Sessao?.Inicio,
            SessaoFim = visualizacao.Sessao?.Fim,
            FestivalId = visualizacao.FestivalId,
            FestivalNome = visualizacao.Festival?.Name ?? string.Empty,
            TipoConteudo = visualizacao.TipoConteudo,
            TipoAcessoUsado = visualizacao.TipoAcessoUsado?.ToString() ?? string.Empty,
            UrlVisualizacao = ObterUrlHistorico(visualizacao),
            VisualizadoEm = visualizacao.VisualizadoEm,
        };
    }

    private static string ObterUrlHistorico(Visualizacao visualizacao)
    {
        return visualizacao.SessaoId.HasValue
            ? $"/visualizar/sessao/{visualizacao.SessaoId.Value}"
            : $"/visualizar/filme/{visualizacao.FilmeId}";
    }
}
