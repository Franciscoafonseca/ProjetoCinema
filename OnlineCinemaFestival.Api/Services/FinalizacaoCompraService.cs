using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FinalizacaoCompraService : IFinalizacaoCompraService
{
    private readonly ICompraRepository _compraRepository;
    private readonly ICarrinhoCheckoutService _carrinhoCheckoutService;
    private readonly ICompraFactory _compraFactory;
    private readonly IAcessoCompraService _acessoCompraService;
    private readonly IPagamentoService _pagamentoService;
    private readonly IEnumerable<ICompraObserver> _compraObservers;
    private readonly int _expiracaoMultibancoHoras;

    public FinalizacaoCompraService(
        ICompraRepository compraRepository,
        ICarrinhoCheckoutService carrinhoCheckoutService,
        ICompraFactory compraFactory,
        IAcessoCompraService acessoCompraService,
        IPagamentoService pagamentoService,
        IEnumerable<ICompraObserver> compraObservers,
        IConfiguration configuration
    )
    {
        _compraRepository = compraRepository;
        _carrinhoCheckoutService = carrinhoCheckoutService;
        _compraFactory = compraFactory;
        _acessoCompraService = acessoCompraService;
        _pagamentoService = pagamentoService;
        _compraObservers = compraObservers;
        _expiracaoMultibancoHoras = PagamentosConfiguracao.ObterExpiracaoMultibancoHoras(
            configuration
        );
    }

    public async Task<ResultadoFinalizacaoCompraDTO> FinalizarCompraAsync(
        int utilizadorId,
        string metodoPagamento
    )
    {
        return await _compraRepository.ExecuteInTransactionAsync(async () =>
        {
            var carrinhoValido = await _carrinhoCheckoutService.ObterCarrinhoValidadoAsync(
                utilizadorId
            );

            var compra = _compraFactory.Criar(utilizadorId, carrinhoValido);

            compra.Pagamento = await _pagamentoService.ProcessarPagamentoSimuladoAsync(compra, metodoPagamento);

            await _compraRepository.AddAsync(compra);

            var pagamentoAprovado = compra.Pagamento.Estado == EstadoPagamento.Aprovado;
            compra.Estado = pagamentoAprovado ? EstadoCompra.Pago : EstadoCompra.Pendente;
            compra.PagaEm = pagamentoAprovado
                ? compra.Pagamento.ProcessadoEm ?? compra.Pagamento.CriadoEm
                : null;

            var acessosGerados = await _acessoCompraService.CriarAcessosSeAprovadoAsync(
                utilizadorId,
                compra,
                carrinhoValido
            );

            if (pagamentoAprovado)
            {
                var acessos = carrinhoValido.Itens.Select(item => item.Acesso).ToList();
                await Task.WhenAll(
                    _compraObservers.Select(observer =>
                        observer.NotificarAsync(utilizadorId, compra.ValorTotal, acessos)
                    )
                );
            }

            await _carrinhoCheckoutService.LimparCarrinhoAsync(carrinhoValido);

            await _compraRepository.SaveChangesAsync();

            var compraCriada = await _compraRepository.ObterPorIdAsync(compra.Id);

            return CompraMapper.MapToCheckoutResultadoDTO(
                compraCriada!,
                acessosGerados,
                pagamentoAprovado
                    ? "Compra finalizada com sucesso."
                    : $"Referencia Multibanco gerada. O pagamento fica pendente durante {_expiracaoMultibancoHoras} horas.",
                _expiracaoMultibancoHoras
            );
        });
    }
}
