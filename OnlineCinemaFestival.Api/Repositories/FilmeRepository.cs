using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class FilmeRepository : IFilmeRepository
{
    private readonly AppDbContext _context;

    public FilmeRepository(AppDbContext context) => _context = context;
    // Busca todos os filmes, sem rastreamento para melhorar a performance
    public async Task<IEnumerable<Filme>> GetAllAsync() => 
        await _context.Filmes.AsNoTracking().ToListAsync();
    // Busca um filme específico pelo id da base de dados
    public async Task<Filme?> GetByIdAsync(int id) => 
        await _context.Filmes.FirstOrDefaultAsync(f => f.Id == id);
    // Busca um filme específico pelo id do TMDb
    public async Task<Filme?> GetByTmdbIdAsync(int tmdbId) => 
        await _context.Filmes.FirstOrDefaultAsync(f => f.TmdbId == tmdbId);
    // Adiciona um novo filme à base de dados
    public async Task AddAsync(Filme filme) => 
        await _context.Filmes.AddAsync(filme);
    // Grava as alterações na base de dados
    public async Task SaveChangesAsync() => 
        await _context.SaveChangesAsync();

}