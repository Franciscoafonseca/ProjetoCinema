using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class FilmeRepository : IFilmeRepository
{
    private readonly AppDbContext _context;

    public FilmeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Filme>> GetAllAsync()
    {
        return await _context
            .Filmes.Include(f => f.FilmeGeneros)
                .ThenInclude(fg => fg.Genero)
            .Include(f => f.Avaliacoes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Filme?> GetByIdAsync(int id)
    {
        return await _context.Filmes.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Filme?> GetDetalheByIdAsync(int id)
    {
        return await _context
            .Filmes.Include(f => f.FilmeGeneros)
                .ThenInclude(fg => fg.Genero)
            .Include(f => f.Avaliacoes)
                .ThenInclude(a => a.Usuario)
            .Include(f => f.FestivalFilmes)
                .ThenInclude(ff => ff.Festival)
            .Include(f => f.SessoesDoFilme)
                .ThenInclude(sf => sf.Sessao)
                    .ThenInclude(s => s.Festival)
            .Include(f => f.Acessos)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Filme?> GetByTmdbIdAsync(int tmdbId)
    {
        return await _context
            .Filmes.Include(f => f.FilmeGeneros)
                .ThenInclude(fg => fg.Genero)
            .Include(f => f.Avaliacoes)
            .FirstOrDefaultAsync(f => f.TmdbId == tmdbId);
    }

    public async Task<List<Filme>> GetTopAsync(int quantidade)
    {
        return await _context
            .Filmes.Include(f => f.FilmeGeneros)
                .ThenInclude(fg => fg.Genero)
            .Include(f => f.Avaliacoes)
            .OrderByDescending(f => f.AvaliacaoTmdb ?? 0)
            .ThenBy(f => f.Titulo)
            .Take(quantidade)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Genero> ObterOuCriarGeneroAsync(string nome)
    {
        var nomeNormalizado = nome.Trim();
        var genero = await _context.Generos.FirstOrDefaultAsync(g => g.Name == nomeNormalizado);

        if (genero != null)
            return genero;

        genero = new Genero { Name = nomeNormalizado, CreatedAt = DateTime.UtcNow };
        await _context.Generos.AddAsync(genero);
        return genero;
    }

    public async Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId)
    {
        return await _context.Visualizacoes.AnyAsync(v =>
            v.UtilizadorId == utilizadorId && v.FilmeId == filmeId
        );
    }

    public async Task<Avaliacao?> GetAvaliacaoAsync(int utilizadorId, int filmeId)
    {
        return await _context.Avaliacoes.FirstOrDefaultAsync(a =>
            a.UsuarioId == utilizadorId && a.FilmeId == filmeId
        );
    }

    public async Task AddAvaliacaoAsync(Avaliacao avaliacao)
    {
        await _context.Avaliacoes.AddAsync(avaliacao);
    }

    public async Task AddAsync(Filme filme)
    {
        await _context.Filmes.AddAsync(filme);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
