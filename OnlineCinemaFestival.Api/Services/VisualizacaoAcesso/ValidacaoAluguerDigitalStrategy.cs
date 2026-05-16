using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public class ValidacaoAluguerDigitalStrategy : IEstrategiaValidacaoAcesso
{
    private readonly AppDbContext _context;

    public ValidacaoAluguerDigitalStrategy(AppDbContext context)
    {
        _context = context;
    }

    public TipoAcesso Tipo => TipoAcesso.AluguerDigital;

    public async Task<AcessoUtilizador?> ValidarFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId,
        DateTime agora
    )
    {
        return await _context
            .AcessosUtilizador.AsNoTracking()
            .Where(a =>
                a.UtilizadorId == utilizadorId
                && a.Ativo
                && a.TipoAcesso == TipoAcesso.AluguerDigital
                && a.FilmeId == filme.Id
                && a.InicioValidade <= agora
                && a.FimValidade >= agora
            )
            .OrderByDescending(a => a.FimValidade)
            .FirstOrDefaultAsync();
    }

    public Task<AcessoUtilizador?> ValidarSessaoAsync(
        int utilizadorId,
        Sessao sessao,
        DateTime agora
    )
    {
        return Task.FromResult<AcessoUtilizador?>(null);
    }
}
