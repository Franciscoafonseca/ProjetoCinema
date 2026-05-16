using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class ListaPessoalRepository : IListaPessoalRepository
{
    private readonly AppDbContext _context;

    public ListaPessoalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ListaPessoal>> GetByUtilizadorAsync(int utilizadorId)
    {
        return await _context
            .ListasPessoais.AsNoTracking()
            .Include(l => l.Items)
                .ThenInclude(i => i.Filme)
            .Where(l => l.UtilizadorId == utilizadorId)
            .OrderBy(l => l.Tipo)
            .ThenBy(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExisteNomeParaUtilizadorAsync(int utilizadorId, string nome)
    {
        var normalizado = nome.Trim().ToLower();

        return await _context.ListasPessoais.AnyAsync(l =>
            l.UtilizadorId == utilizadorId && l.Name.ToLower() == normalizado
        );
    }

    public async Task<ListaPessoal?> ObterPorIdAsync(int id)
    {
        return await _context
            .ListasPessoais.Include(l => l.Items)
                .ThenInclude(i => i.Filme)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task AddAsync(ListaPessoal lista)
    {
        await _context.ListasPessoais.AddAsync(lista);
    }

    public async Task<bool> FilmeExisteAsync(int filmeId)
    {
        return await _context.Filmes.AnyAsync(f => f.Id == filmeId);
    }

    public async Task<ListaPessoalItem?> ObterItemAsync(int listaId, int filmeId)
    {
        return await _context.ListaPessoalItems.FirstOrDefaultAsync(i =>
            i.ListaPessoalId == listaId && i.FilmeId == filmeId
        );
    }

    public async Task AddItemAsync(ListaPessoalItem item)
    {
        await _context.ListaPessoalItems.AddAsync(item);
    }

    public void RemoveItem(ListaPessoalItem item)
    {
        _context.ListaPessoalItems.Remove(item);
    }

    public void Remove(ListaPessoal lista)
    {
        _context.ListasPessoais.Remove(lista);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
