using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.VisualizacaoAcesso;

public class ValidacaoBilheteSessaoStrategy : IEstrategiaValidacaoAcesso
{
    private readonly AppDbContext _context;

    public ValidacaoBilheteSessaoStrategy(AppDbContext context)
    {
        _context = context;
    }

    public TipoAcesso Tipo => TipoAcesso.BilheteSessao;

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
                && a.TipoAcesso == TipoAcesso.BilheteSessao
                && a.SessaoId == sessao.Id
                && a.InicioValidade <= agora
                && a.FimValidade >= agora
            )
            .OrderByDescending(a => a.FimValidade)
            .FirstOrDefaultAsync();
    }
}
