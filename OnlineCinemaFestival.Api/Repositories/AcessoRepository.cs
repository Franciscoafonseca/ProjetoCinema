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
        return await AcessosComDetalhes()
            .AsNoTracking()
            .OrderBy(a => a.Tipo)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Acesso?> ObterPorIdAsync(int id)
    {
        return await AcessosComDetalhes()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Acesso?> ObterBilheteSessaoAtivoAsync(int sessaoId)
    {
        return await ObterAcessosAtivos(TipoAcesso.BilheteSessao)
            .Where(a => a.SessaoId == sessaoId)
            .OrderBy(a => a.Preco)
            .FirstOrDefaultAsync();
    }

    public async Task<Acesso?> ObterPasseDiarioAtivoAsync(int festivalId, DateTime dataAcesso)
    {
        var inicio = dataAcesso.Date;
        var fim = inicio.AddDays(1);

        return await ObterAcessosAtivos(TipoAcesso.PasseDiario)
            .Where(a =>
                a.FestivalId == festivalId
                && a.DataAcesso.HasValue
                && a.DataAcesso.Value >= inicio
                && a.DataAcesso.Value < fim
            )
            .OrderBy(a => a.Preco)
            .FirstOrDefaultAsync();
    }

    public async Task<Acesso?> ObterPasseCompletoAtivoAsync(int festivalId)
    {
        return await ObterAcessosAtivos(TipoAcesso.PasseCompleto)
            .Where(a => a.FestivalId == festivalId)
            .OrderBy(a => a.Preco)
            .FirstOrDefaultAsync();
    }

    public async Task<Acesso?> ObterAluguerDigitalAtivoAsync(int filmeId)
    {
        return await ObterAcessosAtivos(TipoAcesso.AluguerDigital)
            .Where(a => a.FilmeId == filmeId)
            .OrderBy(a => a.Preco)
            .FirstOrDefaultAsync();
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

    private IQueryable<Acesso> AcessosComDetalhes()
    {
        return _context
            .Acessos.AsSplitQuery()
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.Festival)
            .Include(a => a.Sessao)
                .ThenInclude(s => s!.Filme)
            .Include(a => a.Festival)
            .Include(a => a.Filme);
    }

    private IQueryable<Acesso> ObterAcessosAtivos(TipoAcesso tipo)
    {
        return AcessosComDetalhes().Where(a => a.IsAtivo && a.Tipo == tipo);
    }
}
