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

    public async Task<Utilizador?> ObterPorIdAsync(int id)
    {
        return await _context.Utilizadores.Include(u => u.Perfil).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Utilizador?> ObterPorEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _context
            .Utilizadores.Include(u => u.Perfil)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
    }

    public async Task<Utilizador?> ObterPorTelefoneAsync(string telefone)
    {
        var normalizado = telefone.Trim();

        return await _context
            .Utilizadores.Include(u => u.Perfil)
            .FirstOrDefaultAsync(u => u.PhoneNumber == normalizado);
    }

    public async Task<Utilizador?> ObterComPerfilAsync(int id)
    {
        return await UtilizadoresComPerfilCompleto().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<Utilizador>> ObterPerfisPublicosAsync()
    {
        return await UtilizadoresComPerfilCompleto()
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

    private IQueryable<Utilizador> UtilizadoresComPerfilCompleto()
    {
        return _context
            .Utilizadores.AsSplitQuery()
            .Include(u => u.Perfil)
            .Include(u => u.GenerosFavoritos)
                .ThenInclude(ug => ug.Genero)
            .Include(u => u.Avaliacoes)
                .ThenInclude(a => a.Filme)
            .Include(u => u.Comunidades)
                .ThenInclude(c => c.Comunidade)
                    .ThenInclude(c => c.Members)
            .Include(u => u.ListasPessoais)
                .ThenInclude(l => l.Items);
    }
}
