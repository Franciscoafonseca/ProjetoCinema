using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Mapping;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FilmeService : IFilmeService
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly ITmdbService _tmdbService;
    private readonly IAcessoVisualizacaoService _acessoVisualizacaoService;
    private readonly IAcessoAutomaticoService _acessoAutomaticoService;
    private readonly IEnumerable<IAvaliacaoObserver> _avaliacaoObservers;

    public FilmeService(
        IFilmeRepository filmeRepository,
        ITmdbService tmdbService,
        IAcessoVisualizacaoService acessoVisualizacaoService,
        IAcessoAutomaticoService acessoAutomaticoService,
        IEnumerable<IAvaliacaoObserver> avaliacaoObservers
    )
    {
        _filmeRepository = filmeRepository;
        _tmdbService = tmdbService;
        _acessoVisualizacaoService = acessoVisualizacaoService;
        _acessoAutomaticoService = acessoAutomaticoService;
        _avaliacaoObservers = avaliacaoObservers;
    }

    public async Task<IEnumerable<FilmeReadDTO>> ObterTodosFilmesAsync()
    {
        var filmes = await _filmeRepository.ObterTodosAsync();
        return filmes.Select(FilmeMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<FilmeReadDTO>> SearchFilmesTmdbAsync(string query)
    {
        var filmesTmdb = await _tmdbService.SearchFilmesTmdbAsync(query);
        return filmesTmdb.Select(FilmeMapper.MapToReadDTOFromTmdb);
    }

    public async Task<IEnumerable<FilmeReadDTO>> ObterFilmesIniciaisTmdbAsync()
    {
        var filmesTmdb = await _tmdbService.ObterFilmesIniciaisAsync();
        return filmesTmdb.Select(FilmeMapper.MapToReadDTOFromTmdb);
    }

    public async Task<FilmeReadDTO> ImportFilmeFromTmdbAsync(int tmdbId)
    {
        var filmeExistente = await _filmeRepository.ObterPorTmdbIdAsync(tmdbId);

        if (filmeExistente != null)
        {
            await _acessoAutomaticoService.GarantirParaFilmeAsync(filmeExistente.Id);
            var detalheExistente =
                await _filmeRepository.ObterDetalhePorIdAsync(filmeExistente.Id) ?? filmeExistente;

            return FilmeMapper.MapToReadDTO(detalheExistente);
        }

        var filmeTmdb = await _tmdbService.ObterFilmePorTmdbIdAsync(tmdbId);

        if (filmeTmdb == null)
            throw new KeyNotFoundException($"Filme com TMDb ID {tmdbId} nao encontrado.");

        var novoFilme = FilmeMapper.MapFromTmdbDTO(filmeTmdb);

        foreach (var nomeGenero in filmeTmdb.Generos.Where(g => !string.IsNullOrWhiteSpace(g)).Distinct())
        {
            var genero = await _filmeRepository.ObterOuCriarGeneroAsync(nomeGenero);
            novoFilme.FilmeGeneros.Add(new FilmeGenero { Filme = novoFilme, Genero = genero });
        }

        await AdicionarPessoaAsync(novoFilme, filmeTmdb.RealizadorDetalhe, FuncaoPessoaFilme.Realizador, 0);
        await AdicionarPessoaAsync(novoFilme, filmeTmdb.ProdutorDetalhe, FuncaoPessoaFilme.Produtor, 0);

        foreach (var ator in filmeTmdb.AtoresDetalhes)
            await AdicionarPessoaAsync(novoFilme, ator, FuncaoPessoaFilme.Ator, ator.Ordem);

        await _filmeRepository.AddAsync(novoFilme);
        await _filmeRepository.SaveChangesAsync();

        await _acessoAutomaticoService.GarantirParaFilmeAsync(novoFilme.Id);

        return FilmeMapper.MapToReadDTO(novoFilme);
    }

    public async Task<FilmeReadDTO> AtualizarVideoAsync(int filmeId)
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        if (filme.TmdbId <= 0)
            throw new InvalidOperationException("Este filme nao tem identificador TMDB para obter trailer.");

        var videoUrl = await _tmdbService.ObterTrailerUrlAsync(filme.TmdbId);

        if (string.IsNullOrWhiteSpace(videoUrl))
            throw new InvalidOperationException("Trailer TMDB/YouTube indisponivel para este filme.");

        _filmeRepository.AtualizarVideo(
            filme,
            "YouTube",
            ExtrairYouTubeKey(videoUrl),
            videoUrl
        );

        await _filmeRepository.SaveChangesAsync();
        return FilmeMapper.MapToReadDTO(filme);
    }

    public async Task<FilmeDetalheDTO?> ObterDetalheAsync(int filmeId, int? utilizadorId)
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);

        if (filme == null)
            return null;

        var dto = FilmeMapper.MapToReadDTO(filme);

        if (utilizadorId.HasValue)
        {
            dto.PodeAvaliar = await _filmeRepository.UtilizadorViuFilmeAsync(
                utilizadorId.Value,
                filmeId
            );
            dto.PodeVer =
                await _acessoVisualizacaoService.ObterAcessoValidoParaFilmeAsync(
                    utilizadorId.Value,
                    filme,
                    null
                ) != null;
        }

        return dto;
    }

    public async Task<AvaliacaoDTO> CriarReviewAsync(
        int utilizadorId,
        int filmeId,
        CriarAvaliacaoDTO dto
    )
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        if (!await _filmeRepository.UtilizadorViuFilmeAsync(utilizadorId, filmeId))
            throw new UnauthorizedAccessException("So podes avaliar depois de ver o filme.");

        if (await _filmeRepository.ObterAvaliacaoAsync(utilizadorId, filmeId) != null)
            throw new InvalidOperationException("Ja existe uma review tua para este filme.");

        ValidarReview(dto);

        var avaliacao = new Avaliacao
        {
            FilmeId = filmeId,
            UsuarioId = utilizadorId,
            Pontuacao = dto.Pontuacao,
            Texto = dto.Texto.Trim(),
            Data = DateTime.UtcNow,
        };

        await _filmeRepository.AddAvaliacaoAsync(avaliacao);
        await _filmeRepository.SaveChangesAsync();
        await Task.WhenAll(_avaliacaoObservers.Select(observer => observer.NotificarAsync(avaliacao)));

        return MapAvaliacaoDTO(avaliacao, filme.Titulo);
    }

    public async Task<AvaliacaoDTO> AtualizarReviewAsync(
        int utilizadorId,
        int filmeId,
        CriarAvaliacaoDTO dto
    )
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        if (!await _filmeRepository.UtilizadorViuFilmeAsync(utilizadorId, filmeId))
            throw new UnauthorizedAccessException("So podes avaliar depois de ver o filme.");

        var avaliacao = await _filmeRepository.ObterAvaliacaoAsync(utilizadorId, filmeId);

        if (avaliacao == null)
            throw new KeyNotFoundException("Review nao encontrada.");

        ValidarReview(dto);

        avaliacao.Pontuacao = dto.Pontuacao;
        avaliacao.Texto = dto.Texto.Trim();
        avaliacao.Data = DateTime.UtcNow;

        await _filmeRepository.SaveChangesAsync();

        return MapAvaliacaoDTO(avaliacao, filme.Titulo);
    }

    private static void ValidarReview(CriarAvaliacaoDTO dto)
    {
        if (dto.Pontuacao is < 1 or > 10)
            throw new ArgumentException("A pontuacao deve estar entre 1 e 10 estrelas.");

        var texto = dto.Texto?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("A review e obrigatoria.");

        if (texto.Length < 20)
            throw new ArgumentException("A review deve ter pelo menos 20 caracteres.");

        if (texto.Length > 1000)
            throw new ArgumentException("A review nao pode exceder 1000 caracteres.");
    }

    private static AvaliacaoDTO MapAvaliacaoDTO(Avaliacao avaliacao, string tituloFilme)
    {
        return new AvaliacaoDTO
        {
            Id = avaliacao.Id,
            FilmeId = avaliacao.FilmeId,
            TituloFilme = tituloFilme,
            UsuarioId = avaliacao.UsuarioId,
            NomeUsuario = avaliacao.Usuario?.Name ?? string.Empty,
            Pontuacao = avaliacao.Pontuacao,
            Texto = avaliacao.Texto,
            Data = avaliacao.Data,
        };
    }

    private async Task AdicionarPessoaAsync(
        Filme filme,
        TmdbPessoaDTO? pessoaTmdb,
        FuncaoPessoaFilme funcao,
        int ordem
    )
    {
        if (pessoaTmdb == null || string.IsNullOrWhiteSpace(pessoaTmdb.Nome))
            return;

        var pessoa = await _filmeRepository.ObterOuCriarPessoaAsync(
            pessoaTmdb.TmdbPessoaId,
            pessoaTmdb.Nome,
            pessoaTmdb.ImagemUrl
        );

        if (
            filme.PessoasDoFilme.Any(fp =>
                fp.Pessoa == pessoa && fp.Funcao == funcao
            )
        )
            return;

        filme.PessoasDoFilme.Add(
            new FilmePessoa
            {
                Filme = filme,
                Pessoa = pessoa,
                Funcao = funcao,
                Personagem = pessoaTmdb.Personagem,
                Ordem = ordem,
            }
        );
    }

    private static string? ExtrairYouTubeKey(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            if (uri.AbsolutePath.Contains("/embed/", StringComparison.OrdinalIgnoreCase))
                return uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            if (uri.Host.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
                return uri.AbsolutePath.Trim('/');

            var videoId = uri.Query
                .TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(parte => parte.Split('=', 2))
                .FirstOrDefault(partes =>
                    partes.Length == 2 && string.Equals(partes[0], "v", StringComparison.OrdinalIgnoreCase)
                )?[1];

            if (!string.IsNullOrWhiteSpace(videoId))
                return Uri.UnescapeDataString(videoId);
        }

        return null;
    }
}
