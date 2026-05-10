using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Serviço responsável pela associação entre festivais e filmes.
/// Permite associar filmes a festivais, remover associações e consultar os filmes de um festival.
/// </summary>
public class FestivalFilmeService : IFestivalFilmeService
{
    private readonly IFestivalFilmeRepository _festivalFilmeRepository;
    private readonly IFestivalRepository _festivalRepository;
    private readonly IFilmeRepository _filmeRepository;

    /// <summary>
    /// Inicializa uma nova instância do serviço de associação entre festivais e filmes.
    /// </summary>
    /// <param name="festivalFilmeRepository">
    /// Repositório responsável pela gestão das associações entre festivais e filmes.
    /// </param>
    /// <param name="festivalRepository">
    /// Repositório responsável pelo acesso aos dados dos festivais.
    /// </param>
    /// <param name="filmeRepository">
    /// Repositório responsável pelo acesso aos dados dos filmes.
    /// </param>
    public FestivalFilmeService(
        IFestivalFilmeRepository festivalFilmeRepository,
        IFestivalRepository festivalRepository,
        IFilmeRepository filmeRepository
    )
    {
        _festivalFilmeRepository = festivalFilmeRepository;
        _festivalRepository = festivalRepository;
        _filmeRepository = filmeRepository;
    }

    /// <summary>
    /// Associa um filme existente a um festival existente.
    /// </summary>
    /// <param name="festivalId">Identificador do festival.</param>
    /// <param name="filmeId">Identificador do filme.</param>
    /// <exception cref="KeyNotFoundException">
    /// Lançada quando o festival ou o filme não são encontrados.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando o filme já se encontra associado ao festival.
    /// </exception>
    public async Task AssociarFilmeAsync(int festivalId, int filmeId)
    {
        // Verifica se o festival existe antes de criar a associação.
        var festival = await _festivalRepository.GetByIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        // Verifica se o filme existe antes de criar a associação.
        var filme = await _filmeRepository.GetByIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme não encontrado.");

        // Impede associações duplicadas entre o mesmo festival e o mesmo filme.
        var alreadyExists = await _festivalFilmeRepository.ExistsAsync(festivalId, filmeId);

        if (alreadyExists)
            throw new InvalidOperationException("Este filme já está associado a este festival.");

        // Cria a entidade intermédia que representa a relação entre festival e filme.
        var festivalFilme = new FestivalFilme { FestivalId = festivalId, FilmeId = filmeId };

        // Guarda a nova associação na base de dados.
        await _festivalFilmeRepository.AddAsync(festivalFilme);
        await _festivalFilmeRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Remove a associação entre um festival e um filme.
    /// </summary>
    /// <param name="festivalId">Identificador do festival.</param>
    /// <param name="filmeId">Identificador do filme.</param>
    /// <exception cref="KeyNotFoundException">
    /// Lançada quando a associação entre o festival e o filme não é encontrada.
    /// </exception>
    public async Task RemoverFilmeAsync(int festivalId, int filmeId)
    {
        // Procura a associação existente entre o festival e o filme.
        var festivalFilme = await _festivalFilmeRepository.GetAsync(festivalId, filmeId);

        if (festivalFilme == null)
            throw new KeyNotFoundException("Associação entre festival e filme não encontrada.");

        // Remove a associação encontrada.
        _festivalFilmeRepository.Remove(festivalFilme);

        // Confirma a remoção na base de dados.
        await _festivalFilmeRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Obtém todos os filmes associados a um determinado festival.
    /// </summary>
    /// <param name="festivalId">Identificador do festival.</param>
    /// <returns>Lista de filmes associados ao festival indicado.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Lançada quando o festival não é encontrado.
    /// </exception>
    public async Task<IEnumerable<FilmeReadDto>> GetFilmesByFestivalAsync(int festivalId)
    {
        // Verifica se o festival existe antes de procurar os seus filmes.
        var festival = await _festivalRepository.GetByIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        // Obtém os filmes associados ao festival.
        var filmes = await _festivalFilmeRepository.GetFilmesByFestivalIdAsync(festivalId);

        // Converte as entidades Filme para DTOs de leitura antes de devolver a resposta.
        return filmes.Select(FilmeMapper.MapToReadDto);
    }
}
