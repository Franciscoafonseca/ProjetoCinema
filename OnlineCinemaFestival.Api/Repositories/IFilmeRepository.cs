using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IFilmeRepository
{
    // Devolve todos os filmes
    Task<IEnumerable<Filme>> GetAllAsync();
    // Devolve um filme especifico pelo id da Base de Dados
    Task<Filme?> GetByIdAsync(int id);
    // Devolve um filme especifico pelo id do TMDb
    Task<Filme?> GetByTmdbIdAsync(int tmdbId);
    // Adiciona um novo filme à base de dados
    Task AddAsync(Filme filme);
    // grava as alterações na base de dados
    Task SaveChangesAsync();
}
