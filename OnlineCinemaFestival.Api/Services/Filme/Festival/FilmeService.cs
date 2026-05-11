using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Serviço responsável pela gestão dos filmes.
/// Inclui operações de consulta, pesquisa no TMDb e importação de filmes externos.
/// </summary>
public class FilmeService : IFilmeService
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly ITmdbService _tmdbService;

    /// <summary>
    /// Inicializa uma nova instância do serviço de filmes.
    /// </summary>
    /// <param name="filmeRepository">
    /// Repositório responsável pelo acesso aos dados dos filmes guardados localmente.
    /// </param>
    /// <param name="tmdbService">
    /// Serviço responsável pela comunicação com a API externa TMDb.
    /// </param>
    public FilmeService(IFilmeRepository filmeRepository, ITmdbService tmdbService)
    {
        _filmeRepository = filmeRepository;
        _tmdbService = tmdbService;
    }

    /// <summary>
    /// Obtém todos os filmes guardados na base de dados local.
    /// </summary>
    /// <returns>Lista de filmes convertidos para DTOs de leitura.</returns>
    public async Task<IEnumerable<FilmeReadDto>> GetAllFilmesAsync()
    {
        // Obtém todos os filmes existentes na base de dados.
        var filmes = await _filmeRepository.GetAllAsync();

        // Converte as entidades Filme para DTOs de leitura.
        return filmes.Select(FilmeMapper.MapToReadDto);
    }

    /// <summary>
    /// Pesquisa filmes na API externa TMDb com base numa expressão de pesquisa.
    /// </summary>
    /// <param name="query">Texto usado para pesquisar filmes no TMDb.</param>
    /// <returns>Lista de filmes encontrados no TMDb convertidos para DTOs de leitura.</returns>
    public async Task<IEnumerable<FilmeReadDto>> SearchFilmesTmdbAsync(string query)
    {
        // Pesquisa filmes na API externa TMDb.
        var filmesTmdb = await _tmdbService.SearchFilmesTmdbAsync(query);

        // Converte os resultados externos para DTOs compatíveis com a aplicação.
        return filmesTmdb.Select(FilmeMapper.MapToReadDtoFromTmdb);
    }

    /// <summary>
    /// Importa um filme da API TMDb para a base de dados local.
    /// Caso o filme já exista localmente, devolve o registo existente.
    /// </summary>
    /// <param name="tmdbId">Identificador do filme na API TMDb.</param>
    /// <returns>Filme importado ou já existente, convertido para DTO de leitura.</returns>
    /// <exception cref="Exception">
    /// Lançada quando o filme não é encontrado na API TMDb.
    /// </exception>
    public async Task<FilmeReadDto> ImportFilmeFromTmdbAsync(int tmdbId)
    {
        // Verifica se o filme já foi importado anteriormente para evitar duplicações.
        var filmeExistente = await _filmeRepository.GetByTmdbIdAsync(tmdbId);

        if (filmeExistente != null)
            return FilmeMapper.MapToReadDto(filmeExistente);

        // Obtém os dados completos do filme através da API externa TMDb.
        var filmeTmdb = await _tmdbService.GetFilmeByTmdbIdAsync(tmdbId);

        if (filmeTmdb == null)
            throw new Exception($"Filme com TMDb ID {tmdbId} não encontrado.");

        // Converte o DTO recebido do TMDb para a entidade interna Filme.
        var novoFilme = FilmeMapper.MapFromTmdbDto(filmeTmdb);

        // Guarda o novo filme na base de dados local.
        await _filmeRepository.AddAsync(novoFilme);
        await _filmeRepository.SaveChangesAsync();

        // Devolve o filme importado em formato DTO.
        return FilmeMapper.MapToReadDto(novoFilme);
    }
}
