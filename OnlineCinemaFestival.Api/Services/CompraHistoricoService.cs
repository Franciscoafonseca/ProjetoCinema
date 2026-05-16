using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CompraHistoricoService : ICompraHistoricoService
{
    private readonly ICompraRepository _compraRepository;

    public CompraHistoricoService(ICompraRepository compraRepository)
    {
        _compraRepository = compraRepository;
    }

    public Task RegistarAsync(
        string utilizadorId,
        decimal total,
        int pontosGanhos,
        List<CompraHistoricoItemDto> itens)
    {
        var compra = new Compra
        {
            UtilizadorId = utilizadorId,
            Total = total,
            PontosGanhos = pontosGanhos,
            Itens = itens.Select(i => new CompraItem
            {
                TipoAcesso = i.Tipo,
                FilmeId = i.FilmeId,
                SessaoId = i.SessaoId,
                PrecoPago = i.PrecoPago,
                Validade = i.Validade
            }).ToList()
        };

        return GuardarAsync(compra);
    }

    private async Task GuardarAsync(Compra compra)
    {
        await _compraRepository.AddAsync(compra);
        await _compraRepository.SaveChangesAsync();
    }
}
