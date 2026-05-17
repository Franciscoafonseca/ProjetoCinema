using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class AcessoRepository : IAcessoRepository
{
    private readonly AppDbContext _context;

    public AcessoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Acesso>> ObterTodosAsync()
    {
        return await _context
            .Acessos.Include(a => a.Sessao)
                .ThenInclude(s => s!.Festival)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.FilmesDaSessao)
                    .ThenInclude(sf => sf.Filme)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .AsNoTracking()
            .OrderBy(a => a.Tipo)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Acesso?> ObterPorIdAsync(int id)
    {
        return await _context
            .Acessos.Include(a => a.Sessao)
                .ThenInclude(s => s!.Festival)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.FilmesDaSessao)
                    .ThenInclude(sf => sf.Filme)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Acesso?> GetAtivoParaCarrinhoAsync(
        TipoAcesso tipo,
        int? festivalId,
        int? filmeId,
        int? sessaoId,
        DateTime? dataPasse
    )
    {
        var query = _context
            .Acessos.Include(a => a.Sessao)
                .ThenInclude(s => s!.Festival)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.FilmesDaSessao)
                    .ThenInclude(sf => sf.Filme)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .Where(a => a.IsAtivo && a.Tipo == tipo);

        var inicioDataPasse = dataPasse?.Date ?? DateTime.MinValue;
        var fimDataPasse = inicioDataPasse.AddDays(1);

        query = tipo switch
        {
            TipoAcesso.BilheteSessao => query.Where(a => a.SessaoId == sessaoId),
            TipoAcesso.PasseDiario when dataPasse.HasValue =>
                query.Where(a =>
                    a.FestivalId == festivalId
                    && a.DataAcesso.HasValue
                    && a.DataAcesso.Value >= inicioDataPasse
                    && a.DataAcesso.Value < fimDataPasse
                ),
            TipoAcesso.PasseDiario => query.Where(_ => false),
            TipoAcesso.PasseCompleto => query.Where(a => a.FestivalId == festivalId),
            TipoAcesso.AluguerDigital => query.Where(a => a.FilmeId == filmeId),
            _ => query.Where(_ => false),
        };

        return await query.OrderBy(a => a.Preco).FirstOrDefaultAsync();
    }

    public async Task AddAsync(Acesso acesso)
    {
        await _context.Acessos.AddAsync(acesso);
    }

    public async Task AddManyAsync(IEnumerable<Acesso> acessos)
    {
        await _context.Acessos.AddRangeAsync(acessos);
    }

    public void Remove(Acesso acesso)
    {
        _context.Acessos.Remove(acesso);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
