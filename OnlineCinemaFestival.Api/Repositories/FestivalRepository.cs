using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class FestivalRepository : IFestivalRepository
{
    private readonly AppDbContext _context;

    public FestivalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Festival>> GetAllAsync()
    {
        return await _context.Festivals.AsNoTracking().OrderBy(f => f.StartDate).ToListAsync();
    }

    public async Task<Festival?> GetByIdAsync(int id)
    {
        return await _context.Festivals.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task AddAsync(Festival festival)
    {
        await _context.Festivals.AddAsync(festival);
    }

    public void Remove(Festival festival)
    {
        _context.Festivals.Remove(festival);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
