using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class PagamentosPendentesService : IPagamentosPendentesService
{
    private readonly ICompraRepository _compraRepository;
    private readonly int _expiracaoMultibancoHoras;
    private readonly TimeProvider _timeProvider;

    public PagamentosPendentesService(
        ICompraRepository compraRepository,
        IConfiguration configuration,
        TimeProvider timeProvider
    )
    {
        _compraRepository = compraRepository;
        _timeProvider = timeProvider;
        _expiracaoMultibancoHoras = PagamentosConfiguracao.ObterExpiracaoMultibancoHoras(
            configuration
        );
    }

    public async Task<List<CompraReadDTO>> ObterDoUtilizadorAsync(int utilizadorId)
    {
        await ExpirarPendentesAsync();

        var compras = await _compraRepository.ObterPagamentosMultibancoPorUtilizadorAsync(
            utilizadorId
        );

        return compras.Select(c => CompraMapper.MapToReadDTO(c, _expiracaoMultibancoHoras)).ToList();
    }

    public async Task<int> ExpirarPendentesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var compras = await _compraRepository.ObterPagamentosMultibancoPendentesAsync();
        var agora = AgoraUtc();
        var expirados = 0;

        foreach (var compra in compras)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (compra.Pagamento == null || !ReferenciaExpirada(compra.Pagamento, agora))
                continue;

            compra.Pagamento.Estado = EstadoPagamento.Expirado;
            compra.Pagamento.ProcessadoEm = agora;
            compra.Pagamento.Mensagem = "Referencia Multibanco expirada.";
            compra.Estado = EstadoCompra.Cancelado;
            expirados++;
        }

        if (expirados > 0)
            await _compraRepository.SaveChangesAsync();

        return expirados;
    }

    public async Task<CompraReadDTO> ConfirmarPagamentoAsync(int compraId, int utilizadorId)
    {
        await ExpirarPendentesAsync();

        var compra = await _compraRepository.ObterPorIdAsync(compraId);
        if (compra == null || compra.UtilizadorId != utilizadorId)
            throw new KeyNotFoundException("Pagamento pendente nao encontrado.");

        if (compra.Pagamento == null)
            throw new InvalidOperationException("Compra sem pagamento associado.");

        if (compra.Pagamento.Estado == EstadoPagamento.Expirado)
            throw new InvalidOperationException("A referencia expirou. Cria uma nova compra para pagar.");

        if (compra.Pagamento.Estado != EstadoPagamento.Pendente)
            throw new InvalidOperationException("Este pagamento ja nao esta pendente.");

        if (ReferenciaExpirada(compra.Pagamento, AgoraUtc()))
            throw new InvalidOperationException("A referencia expirou. Cria uma nova compra para pagar.");

        throw new InvalidOperationException(
            "A referencia Multibanco e ficticia e nao e confirmada automaticamente."
        );
    }

    private bool ReferenciaExpirada(Pagamento pagamento, DateTime agoraUtc)
    {
        return pagamento.CriadoEm.AddHours(_expiracaoMultibancoHoras) <= agoraUtc;
    }

    private DateTime AgoraUtc() => _timeProvider.GetUtcNow().UtcDateTime;
}
