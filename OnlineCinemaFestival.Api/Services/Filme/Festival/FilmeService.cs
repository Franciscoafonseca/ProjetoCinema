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

    public async Task<IEnumerable<FilmeReadDto>> GetAllFilmesAsync()
    {
        var filmes = await _filmeRepository.GetAllAsync();
        return filmes.Select(FilmeMapper.MapToReadDto);
    }

    public async Task<IEnumerable<FilmeReadDto>> SearchFilmesTmdbAsync(string query)
    {
        var filmesTmdb = await _tmdbService.SearchFilmesTmdbAsync(query);
        return filmesTmdb.Select(FilmeMapper.MapToReadDtoFromTmdb);
    }

    public async Task<IEnumerable<FilmeReadDto>> GetFilmesIniciaisTmdbAsync()
    {
        var filmesTmdb = await _tmdbService.GetFilmesIniciaisAsync();
        return filmesTmdb.Select(FilmeMapper.MapToReadDtoFromTmdb);
    }

    public async Task<FilmeReadDto> ImportFilmeFromTmdbAsync(int tmdbId)
    {
        var filmeExistente = await _filmeRepository.GetByTmdbIdAsync(tmdbId);

        if (filmeExistente != null)
            return FilmeMapper.MapToReadDto(filmeExistente);

        var filmeTmdb = await _tmdbService.GetFilmeByTmdbIdAsync(tmdbId);

        if (filmeTmdb == null)
            throw new KeyNotFoundException($"Filme com TMDb ID {tmdbId} nao encontrado.");

        var novoFilme = FilmeMapper.MapFromTmdbDto(filmeTmdb);

        foreach (var nomeGenero in filmeTmdb.Generos.Where(g => !string.IsNullOrWhiteSpace(g)).Distinct())
        {
            var genero = await _filmeRepository.ObterOuCriarGeneroAsync(nomeGenero);
            novoFilme.FilmeGeneros.Add(new FilmeGenero { Filme = novoFilme, Genero = genero });
        }

        await _filmeRepository.AddAsync(novoFilme);
        await _filmeRepository.SaveChangesAsync();

        return FilmeMapper.MapToReadDto(novoFilme);
    }

    public async Task<FilmeDetalheDto?> GetDetalheAsync(int filmeId, int? utilizadorId)
    {
        var filme = await _filmeRepository.GetDetalheByIdAsync(filmeId);

        if (filme == null)
            return null;

        var dto = FilmeMapper.MapToReadDto(filme);

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

    public async Task<AvaliacaoDto> CriarReviewAsync(
        int utilizadorId,
        int filmeId,
        CriarAvaliacaoDto dto
    )
    {
        var filme = await _filmeRepository.GetDetalheByIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        if (!await _filmeRepository.UtilizadorViuFilmeAsync(utilizadorId, filmeId))
            throw new UnauthorizedAccessException("So podes avaliar depois de ver o filme.");

        if (await _filmeRepository.GetAvaliacaoAsync(utilizadorId, filmeId) != null)
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

        return new AvaliacaoDto
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
