using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class UtilizadorRepository : IUtilizadorRepository
{
    private readonly AppDbContext _context;

    public UtilizadorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Utilizador?> GetByIdAsync(int id)
    {
        return await _context.Utilizadores.FindAsync(id);
    }

    public async Task<Utilizador?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _context
            .Utilizadores.Include(u => u.Perfil)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
    }

    public async Task<Utilizador?> GetWithProfileAsync(int id)
    {
        return await _context
            .Utilizadores.Include(u => u.Perfil)
            .Include(u => u.GenerosFavoritos)
                .ThenInclude(ug => ug.Genero)
            .Include(u => u.Avaliacoes)
            .Include(u => u.Comunidades)
            .Include(u => u.ListasPessoais)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<Utilizador>> GetPublicProfilesAsync()
    {
        return await _context
            .Utilizadores.Include(u => u.Perfil)
            .Include(u => u.GenerosFavoritos)
                .ThenInclude(ug => ug.Genero)
            .Include(u => u.Avaliacoes)
            .Include(u => u.Comunidades)
            .Include(u => u.ListasPessoais)
            .Where(u => u.IsActive && u.Perfil != null && u.Perfil.IsPublic)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Utilizador utilizador)
    {
        _context.Utilizadores.Add(utilizador);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
