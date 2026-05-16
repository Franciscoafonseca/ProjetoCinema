using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class CompraRepository : ICompraRepository
{
    private readonly AppDbContext _context;

    public CompraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Compra compra)
    {
        await _context.Compras.AddAsync(compra);
    }

    public async Task<List<Compra>> GetByUtilizadorAsync(string utilizadorId)
    {
        return await _context.Compras
            .Include(c => c.Itens)
            .Where(c => c.UtilizadorId == utilizadorId)
            .OrderByDescending(c => c.Data)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
