using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public class ValidacaoPasseDiarioStrategy : IEstrategiaValidacaoAcesso
{
    private readonly AppDbContext _context;

    public ValidacaoPasseDiarioStrategy(AppDbContext context)
    {
        _context = context;
    }

    public TipoAcesso Tipo => TipoAcesso.PasseDiario;

    public Task<AcessoUtilizador?> ValidarFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId,
        DateTime agora
    )
    {
        return Task.FromResult<AcessoUtilizador?>(null);
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
                && a.TipoAcesso == TipoAcesso.PasseDiario
                && a.FestivalId == sessao.FestivalId
                && a.InicioValidade <= agora
                && a.FimValidade >= agora
                && sessao.Inicio >= a.InicioValidade
                && sessao.Inicio < a.FimValidade
            )
            .OrderByDescending(a => a.FimValidade)
            .FirstOrDefaultAsync();
    }
}
