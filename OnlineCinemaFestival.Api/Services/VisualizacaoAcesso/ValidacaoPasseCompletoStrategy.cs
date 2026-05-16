using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public class ValidacaoPasseCompletoStrategy : IEstrategiaValidacaoAcesso
{
    private readonly AppDbContext _context;

    public ValidacaoPasseCompletoStrategy(AppDbContext context)
    {
        _context = context;
    }

    public TipoAcesso Tipo => TipoAcesso.PasseCompleto;

    public async Task<AcessoUtilizador?> ValidarFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId,
        DateTime agora
    )
    {
        var query = _context
            .AcessosUtilizador.AsNoTracking()
            .Where(a =>
                a.UtilizadorId == utilizadorId
                && a.Ativo
                && a.TipoAcesso == TipoAcesso.PasseCompleto
                && a.FestivalId != null
                && a.InicioValidade <= agora
                && a.FimValidade >= agora
            );

        if (festivalId.HasValue)
            query = query.Where(a => a.FestivalId == festivalId.Value);

        return await query
            .Where(a =>
                _context.FestivalFilmes.Any(ff =>
                    ff.FestivalId == a.FestivalId && ff.FilmeId == filme.Id
                )
            )
            .OrderByDescending(a => a.FimValidade)
            .FirstOrDefaultAsync();
    }

    public async Task<AcessoUtilizador?> ValidarSessaoAsync(
        int utilizadorId,
        Sessao sessao,
        DateTime agora
    )
    {
        return await _context
            .AcessosUtilizador.AsNoTracking()
            .Where(a =>
                a.UtilizadorId == utilizadorId
                && a.Ativo
                && a.TipoAcesso == TipoAcesso.PasseCompleto
                && a.FestivalId == sessao.FestivalId
                && a.InicioValidade <= agora
                && a.FimValidade >= agora
            )
            .OrderByDescending(a => a.FimValidade)
            .FirstOrDefaultAsync();
    }
}
