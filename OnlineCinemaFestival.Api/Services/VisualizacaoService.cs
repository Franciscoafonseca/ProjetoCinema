using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class VisualizacaoService : IVisualizacaoService
{
    private readonly IVisualizacaoRepository _visualizacaoRepository;
    private readonly IValidacaoAcessoService _validacaoAcessoService;
    private readonly IEnumerable<IVisualizacaoObserver> _observers;

    public VisualizacaoService(
        IVisualizacaoRepository visualizacaoRepository,
        IValidacaoAcessoService validacaoAcessoService,
        IEnumerable<IVisualizacaoObserver> observers
    )
    {
        _visualizacaoRepository = visualizacaoRepository;
        _validacaoAcessoService = validacaoAcessoService;
        _observers = observers;
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

        var url = await ObterUrlVisualizacaoAsync(sessao.Filme);
        var conteudos = new List<ConteudoVisualizacaoDTO>
        {
            new()
            {
                FilmeId = sessao.FilmeId,
                Titulo = sessao.Filme.Titulo,
                PosterUrl = sessao.Filme.CapaUrl,
                Ordem = 1,
                UrlVisualizacao = url,
            },
        };

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

            if (sessao.FilmeId != dto.FilmeId)
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
        await NotificarVisualizacaoAsync(visualizacao);

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
        if (!string.IsNullOrWhiteSpace(filme.TrailerUrl))
            return filme.TrailerUrl;

        if (!string.IsNullOrWhiteSpace(filme.VideoUrl))
            return filme.VideoUrl;

        throw new InvalidOperationException("Trailer TMDB/YouTube indisponivel para este filme.");
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
        var visualizacao = new Visualizacao
        {
            UtilizadorId = utilizadorId,
            FilmeId = filmeId,
            SessaoId = sessaoId,
            FestivalId = festivalId,
            TipoConteudo = tipoConteudo,
            TipoAcessoUsado = tipoAcessoUsado,
            UrlVisualizacao = urlVisualizacao,
            VisualizadoEm = DateTime.UtcNow,
        };

        await _visualizacaoRepository.AddAsync(visualizacao);

        await _visualizacaoRepository.SaveChangesAsync();
        await NotificarVisualizacaoAsync(visualizacao);
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

        var lista = visualizacoes.ToList();
        await _visualizacaoRepository.AddRangeAsync(lista);
        await _visualizacaoRepository.SaveChangesAsync();
        await Task.WhenAll(lista.Select(NotificarVisualizacaoAsync));
    }

    private Task NotificarVisualizacaoAsync(Visualizacao visualizacao)
    {
        return Task.WhenAll(_observers.Select(observer => observer.NotificarAsync(visualizacao)));
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
