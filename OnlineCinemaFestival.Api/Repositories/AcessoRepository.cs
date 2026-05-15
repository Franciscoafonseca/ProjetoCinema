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

    public async Task<IEnumerable<Acesso>> GetAllAsync()
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

    public async Task<Acesso?> GetByIdAsync(int id)
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

        query = tipo switch
        {
            TipoAcesso.BilheteSessao => query.Where(a => a.SessaoId == sessaoId),
            TipoAcesso.PasseDiario => query.Where(a =>
                a.FestivalId == festivalId && a.DataAcesso.HasValue
            ),
            TipoAcesso.PasseCompleto => query.Where(a => a.FestivalId == festivalId),
            TipoAcesso.AluguerDigital => query.Where(a => a.FilmeId == filmeId),
            _ => query.Where(_ => false),
        };

        var acessos = (await query.ToListAsync()).OrderBy(a => a.Preco).ToList();

        if (tipo == TipoAcesso.PasseDiario && dataPasse.HasValue)
            return acessos.FirstOrDefault(a => a.DataAcesso?.Date == dataPasse.Value.Date);

        return acessos.FirstOrDefault();
    }

    public async Task AddAsync(Acesso acesso)
    {
        await _context.Acessos.AddAsync(acesso);
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
