using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Mappers;

namespace OnlineCinemaFestival.Api.Services;

public class FilmeService : IFilmeService
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly ITmdbService _tmdbService;

    public FilmeService(IFilmeRepository filmeRepository, ITmdbService tmdbService)
    {
        _filmeRepository = filmeRepository;
        _tmdbService = tmdbService;
    }

    public async Task<IEnumerable<FilmeReadDto>> GetAllFilmesAsync()
    {
        var filmes = await _filmeRepository.GetAllAsync();
        return filmes.Select(FilmeMapper.MapToReadDto);
    }

    public async Task<IEnumerable<FilmeReadDto>> SearchFilmesTmdbAsync(string query)
    {
        var filmesTmdb = await _tmdbService.SearchFilmesTmbdAsync(query);
        return filmesTmdb.Select(FilmeMapper.MapToReadDto);
    }

    public async Task<FilmeReadDto> ImportFilmeFromTmdbAsync(int tmdbId)
    {
        // Verificar se o filme já existe na base de dados
        var filmeExistente = await _filmeRepository.GetByTmdbIdAsync(tmdbId);
        if (filmeExistente != null) return FilmeMapper.MapToReadDto(filmeExistente);
    

        // Buscar os dados do filme no TMDb
        var filmeTmdb = await _tmdbService.GetFilmeByTmdbIdAsync(tmdbId);
        if (filmeTmdb == null) throw new Exception($"Filme com TMDb ID {tmdbId} não encontrado.");
        

        // Criar um novo filme com os dados do TMDb
        var novoFilme = FilmeMapper.MapFromTmdbDto(filmeTmdb);

        // Adicionar o novo filme à base de dados
        await _filmeRepository.AddAsync(novoFilme);
        await _filmeRepository.SaveChangesAsync();

        return FilmeMapper.MapToReadDto(novoFilme);
        
    }
    
}