using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CompraService : ICompraService
{
    private readonly ICompraRepository _compraRepository;
    private readonly int _expiracaoMultibancoHoras;

    public CompraService(ICompraRepository compraRepository, IConfiguration configuration)
    {
        _compraRepository = compraRepository;
        _expiracaoMultibancoHoras = PagamentosConfiguracao.ObterExpiracaoMultibancoHoras(
            configuration
        );
    }

    public async Task<IEnumerable<CompraReadDTO>> ObterComprasDoUtilizadorAsync(int utilizadorId)
    {
        var compras = await _compraRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        return compras.Select(compra =>
            CompraMapper.MapToReadDTO(compra, _expiracaoMultibancoHoras)
        );
    }

    public async Task<IEnumerable<CompraHistoricoReadDto>> ObterHistoricoDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var compras = await _compraRepository.ObterHistoricoPorUtilizadorAsync(utilizadorId);

        return compras.Select(c => new CompraHistoricoReadDto
        {
            Id = c.Id,
            UtilizadorId = c.UtilizadorId,
            Data = c.CriadaEm,
            Total = c.ValorTotal,
            Estado = (int)c.Estado,
            EstadoNome = c.Estado.ToString(),
            PontosGanhos = c.Estado == EstadoCompra.Pago ? (int)(c.ValorTotal / 10) : 0,
            Itens = c
                .Itens.Select(i => new CompraHistoricoItemReadDto
                {
                    Id = i.Id,
                    FilmeId = i.Acesso.FilmeId,
                    SessaoId = i.Acesso.SessaoId,
                    TipoAcesso = (int)i.TipoAcesso,
                    PrecoPago = i.Subtotal,
                    Validade = null,
                })
                .ToList(),
        });
    }
}
