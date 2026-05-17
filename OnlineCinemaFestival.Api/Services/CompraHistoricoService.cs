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
        int utilizadorId,
        decimal total,
        int pontosGanhos,
        List<CompraHistoricoItemDto> itens
    )
    {
        var agora = DateTime.UtcNow;
        var compra = new Compra
        {
            UtilizadorId = utilizadorId,
            Referencia = $"HIST-{Guid.NewGuid():N}"[..13].ToUpperInvariant(),
            CriadaEm = agora,
            Estado = EstadoCompra.Pago,
            PagaEm = agora,
            ValorTotal = total,
            Itens = itens
                .Select(i => new ItemCompra
                {
                    NomeAcesso = i.Tipo.ToString(),
                    TipoAcesso = i.Tipo,
                    PrecoUnitario = i.PrecoPago,
                    Quantidade = 1,
                    Subtotal = i.PrecoPago,
                })
                .ToList(),
        };

        return GuardarAsync(compra);
    }

    private async Task GuardarAsync(Compra compra)
    {
        await _compraRepository.AddAsync(compra);
        await _compraRepository.SaveChangesAsync();
    }
}
