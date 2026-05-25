using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Mapping;
using OnlineCinemaFestival.Api.Domain;
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
        IOptions<PagamentoOptions> pagamentoOptions
    )
    {
        _compraRepository = compraRepository;
        _carrinhoCheckoutService = carrinhoCheckoutService;
        _compraFactory = compraFactory;
        _acessoCompraService = acessoCompraService;
        _pagamentoService = pagamentoService;
        _compraObservers = compraObservers;
        _expiracaoMultibancoHoras = pagamentoOptions.Value.Multibanco.ExpiracaoHoras;
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

            compra.Pagamento = await _pagamentoService.ProcessarPagamentoSimuladoAsync(
                compra,
                metodoPagamento
            );

            await _compraRepository.AddAsync(compra);

            var estadoPagamento = compra.Pagamento.Estado;
            var pagamentoAprovado = estadoPagamento == EstadoPagamento.Aprovado;
            compra.Estado = ResolverEstadoCompra(estadoPagamento);
            compra.PagaEm = pagamentoAprovado
                ? compra.Pagamento.ProcessadoEm ?? compra.Pagamento.CriadoEm
                : null;

            var acessosGerados = await _acessoCompraService.CriarAcessosSeAprovadoAsync(
                utilizadorId,
                compra,
                carrinhoValido
            );

            if (pagamentoAprovado || estadoPagamento == EstadoPagamento.Pendente)
            {
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
            }

            await _compraRepository.SaveChangesAsync();

            var compraCriada = await _compraRepository.ObterPorIdAsync(compra.Id);

            return CompraMapper.MapToCheckoutResultadoDTO(
                compraCriada!,
                acessosGerados,
                ObterMensagemResultado(estadoPagamento),
                _expiracaoMultibancoHoras
            );
        });
    }

    private EstadoCompra ResolverEstadoCompra(EstadoPagamento estadoPagamento)
    {
        return estadoPagamento switch
        {
            EstadoPagamento.Aprovado => EstadoCompra.Pago,
            EstadoPagamento.Pendente => EstadoCompra.Pendente,
            EstadoPagamento.Recusado => EstadoCompra.Cancelado,
            _ => EstadoCompra.Cancelado,
        };
    }

    private string ObterMensagemResultado(EstadoPagamento estadoPagamento)
    {
        return estadoPagamento switch
        {
            EstadoPagamento.Aprovado => "Compra finalizada com sucesso.",
            EstadoPagamento.Pendente =>
                $"Referencia Multibanco gerada. O pagamento fica pendente durante {_expiracaoMultibancoHoras} horas.",
            EstadoPagamento.Recusado => "Pagamento recusado. A compra foi cancelada.",
            EstadoPagamento.Expirado => "Referencia Multibanco expirada.",
            _ => "Pagamento nao aprovado. A compra foi cancelada.",
        };
    }
}
