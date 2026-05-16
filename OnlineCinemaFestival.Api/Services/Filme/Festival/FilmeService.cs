using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FilmeService : IFilmeService
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly ITmdbService _tmdbService;
    private readonly IValidacaoAcessoService _validacaoAcessoService;

    public FilmeService(
        IFilmeRepository filmeRepository,
        ITmdbService tmdbService,
        IValidacaoAcessoService validacaoAcessoService
    )
    {
        _filmeRepository = filmeRepository;
        _tmdbService = tmdbService;
        _validacaoAcessoService = validacaoAcessoService;
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
            return FilmeMapper.MapToReadDTO(filmeExistente);

        var filmeTmdb = await _tmdbService.ObterFilmePorTmdbIdAsync(tmdbId);

        if (filmeTmdb == null)
            throw new KeyNotFoundException($"Filme com TMDb ID {tmdbId} nao encontrado.");

        var novoFilme = FilmeMapper.MapFromTmdbDTO(filmeTmdb);

        foreach (var nomeGenero in filmeTmdb.Generos.Where(g => !string.IsNullOrWhiteSpace(g)).Distinct())
        {
            var genero = await _filmeRepository.ObterOuCriarGeneroAsync(nomeGenero);
            novoFilme.FilmeGeneros.Add(new FilmeGenero { Filme = novoFilme, Genero = genero });
        }

        await _filmeRepository.AddAsync(novoFilme);
        await _filmeRepository.SaveChangesAsync();

        return FilmeMapper.MapToReadDTO(novoFilme);
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
                await _validacaoAcessoService.ObterAcessoValidoParaFilmeAsync(
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

        return new AvaliacaoDTO
        {
            Id = avaliacao.Id,
            FilmeId = filmeId,
            TituloFilme = filme.Titulo,
            UsuarioId = utilizadorId,
            Pontuacao = avaliacao.Pontuacao,
            Texto = avaliacao.Texto,
            Data = avaliacao.Data,
        };
    }
}
