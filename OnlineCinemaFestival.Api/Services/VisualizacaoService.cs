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

        var url = await ObterUrlVisualizacaoAsync(filme);

        await RegistarVisualizacaoInternaAsync(
            utilizadorId,
            filme.Id,
            null,
            festivalId,
            "Filme",
            url
        );

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

            var url = await ObterUrlVisualizacaoAsync(filme);

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

        await RegistarVisualizacoesSessaoAsync(
            utilizadorId,
            sessao.Id,
            sessao.FestivalId,
            conteudos
        );

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

    public async Task<IEnumerable<VisualizacaoHistoricoReadDto>> ObterHistoricoDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var visualizacoes = await _visualizacaoRepository.GetByUtilizadorIdAsync(utilizadorId);

        return visualizacoes.Select(MapToHistoricoDto);
    }

    public async Task<VisualizacaoHistoricoReadDto> RegistarVisualizacaoAsync(
        int utilizadorId,
        RegistarVisualizacaoDto dto
    )
    {
        var filme = await _visualizacaoRepository.GetFilmeByIdAsync(dto.FilmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        var tipoConteudo = "Filme";
        int? festivalId = dto.FestivalId;

        if (dto.SessaoId.HasValue)
        {
            var sessao = await _visualizacaoRepository.GetSessaoByIdAsync(dto.SessaoId.Value);

            if (sessao == null)
                throw new KeyNotFoundException("Sessao nao encontrada.");

            if (!sessao.FilmesDaSessao.Any(sf => sf.FilmeId == dto.FilmeId))
                throw new InvalidOperationException("O filme nao pertence a esta sessao.");

            var podeVisualizarSessao = await _validacaoAcessoService.PodeVisualizarSessaoAsync(
                utilizadorId,
                sessao
            );

            if (!podeVisualizarSessao)
                throw new UnauthorizedAccessException("Sem acesso valido para esta sessao.");

            tipoConteudo = "Sessao";
            festivalId = sessao.FestivalId;
        }
        else
        {
            var podeVisualizarFilme = await _validacaoAcessoService.PodeVisualizarFilmeAsync(
                utilizadorId,
                dto.FilmeId,
                dto.FestivalId
            );

            if (!podeVisualizarFilme)
                throw new UnauthorizedAccessException("Sem acesso valido para este filme.");
        }

        var url = string.IsNullOrWhiteSpace(dto.UrlVisualizacao)
            ? await ObterUrlVisualizacaoAsync(filme)
            : dto.UrlVisualizacao;

        var visualizacao = new Visualizacao
        {
            UtilizadorId = utilizadorId,
            FilmeId = filme.Id,
            SessaoId = dto.SessaoId,
            FestivalId = festivalId,
            TipoConteudo = tipoConteudo,
            UrlVisualizacao = url,
            VisualizadoEm = DateTime.UtcNow,
        };

        await _visualizacaoRepository.AddAsync(visualizacao);
        await _visualizacaoRepository.SaveChangesAsync();

        return new VisualizacaoHistoricoReadDto
        {
            Id = visualizacao.Id,
            FilmeId = filme.Id,
            FilmeTitulo = filme.Titulo,
            SessaoId = visualizacao.SessaoId,
            FestivalId = visualizacao.FestivalId,
            TipoConteudo = visualizacao.TipoConteudo,
            UrlVisualizacao = visualizacao.UrlVisualizacao,
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

        var trailerUrl = await _tmdbService.GetTrailerUrlAsync(filme.TmdbId);

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
        IEnumerable<ConteudoVisualizacaoDto> conteudos
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
            UrlVisualizacao = conteudo.UrlVisualizacao,
            VisualizadoEm = agora,
        });

        await _visualizacaoRepository.AddRangeAsync(visualizacoes);
        await _visualizacaoRepository.SaveChangesAsync();
    }

    private static VisualizacaoHistoricoReadDto MapToHistoricoDto(Visualizacao visualizacao)
    {
        return new VisualizacaoHistoricoReadDto
        {
            Id = visualizacao.Id,
            FilmeId = visualizacao.FilmeId,
            FilmeTitulo = visualizacao.Filme?.Titulo ?? string.Empty,
            SessaoId = visualizacao.SessaoId,
            FestivalId = visualizacao.FestivalId,
            FestivalNome = visualizacao.Festival?.Name ?? string.Empty,
            TipoConteudo = visualizacao.TipoConteudo,
            UrlVisualizacao = visualizacao.UrlVisualizacao,
            VisualizadoEm = visualizacao.VisualizadoEm,
        };
    }
}
