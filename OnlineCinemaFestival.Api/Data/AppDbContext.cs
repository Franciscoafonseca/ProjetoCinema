using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Festival> Festivals => Set<Festival>();
    public DbSet<Acesso> Acessos => Set<Acesso>();
    public DbSet<Reward> Rewards => Set<Reward>();
}
