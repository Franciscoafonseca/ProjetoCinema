using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class VisualizacaoRepository : IVisualizacaoRepository
{
    private readonly AppDbContext _context;

    public VisualizacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Filme?> GetFilmeByIdAsync(int filmeId)
    {
        return await _context.Filmes.AsNoTracking().FirstOrDefaultAsync(f => f.Id == filmeId);
    }

    public async Task<Sessao?> GetSessaoByIdAsync(int sessaoId)
    {
        return await _context
            .Sessoes.Include(s => s.Festival)
            .Include(s => s.FilmesDaSessao)
                .ThenInclude(sf => sf.Filme)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessaoId);
    }

    public async Task<bool> FilmePertenceAoFestivalAsync(int filmeId, int festivalId)
    {
        return await _context.FestivalFilmes.AnyAsync(ff =>
            ff.FilmeId == filmeId && ff.FestivalId == festivalId
        );
    }

    public async Task<bool> TemAcessoAtivoParaFilmeAsync(
        int utilizadorId,
        int filmeId,
        DateTime agora
    )
    {
        return await _context.AcessosUtilizador.AnyAsync(a =>
            a.UtilizadorId == utilizadorId
            && a.Ativo
            && a.FilmeId == filmeId
            && a.TipoAcesso == TipoAcesso.AluguerDigital
            && a.InicioValidade <= agora
            && a.FimValidade >= agora
        );
    }

    public async Task<bool> TemPasseAtivoParaFilmeNoFestivalAsync(
        int utilizadorId,
        int filmeId,
        int festivalId,
        DateTime agora
    )
    {
        var filmePertenceAoFestival = await FilmePertenceAoFestivalAsync(filmeId, festivalId);

        if (!filmePertenceAoFestival)
            return false;

        return await _context.AcessosUtilizador.AnyAsync(a =>
            a.UtilizadorId == utilizadorId
            && a.Ativo
            && a.FestivalId == festivalId
            && (a.TipoAcesso == TipoAcesso.PasseDiario || a.TipoAcesso == TipoAcesso.PasseCompleto)
            && a.InicioValidade <= agora
            && a.FimValidade >= agora
        );
    }

    public async Task<bool> TemAcessoAtivoParaSessaoAsync(
        int utilizadorId,
        Sessao sessao,
        DateTime agora
    )
    {
        return await _context.AcessosUtilizador.AnyAsync(a =>
            a.UtilizadorId == utilizadorId
            && a.Ativo
            && a.InicioValidade <= agora
            && a.FimValidade >= agora
            && (
                (a.TipoAcesso == TipoAcesso.BilheteSessao && a.SessaoId == sessao.Id)
                || (
                    a.TipoAcesso == TipoAcesso.PasseDiario
                    && a.FestivalId == sessao.FestivalId
                    && sessao.Inicio >= a.InicioValidade
                    && sessao.Inicio < a.FimValidade
                )
                || (a.TipoAcesso == TipoAcesso.PasseCompleto && a.FestivalId == sessao.FestivalId)
            )
        );
    }

    public async Task AddAsync(Visualizacao visualizacao)
    {
        await _context.Visualizacoes.AddAsync(visualizacao);
    }

    public async Task AddRangeAsync(IEnumerable<Visualizacao> visualizacoes)
    {
        await _context.Visualizacoes.AddRangeAsync(visualizacoes);
    }

    public async Task<IEnumerable<Visualizacao>> GetByUtilizadorIdAsync(int utilizadorId)
    {
        return await _context
            .Visualizacoes.Where(v => v.UtilizadorId == utilizadorId)
            .Include(v => v.Filme)
            .Include(v => v.Sessao)
            .Include(v => v.Festival)
            .OrderByDescending(v => v.VisualizadoEm)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
