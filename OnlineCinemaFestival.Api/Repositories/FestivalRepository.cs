using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class FestivalRepository
{
    private readonly AppDbContext _context;

    public FestivalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Festival>> GetAllAsync()
    {
        return await _context.Festivals.OrderBy(f => f.StartDate).ToListAsync();
    }

    public async Task<Festival?> GetByIdAsync(int id)
    {
        return await _context.Festivals.FindAsync(id);
    }

    public async Task AddAsync(Festival festival)
    {
        _context.Festivals.Add(festival);
        await _context.SaveChangesAsync();
    }
}
