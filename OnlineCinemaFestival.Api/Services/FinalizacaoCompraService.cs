using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FinalizacaoCompraService : IFinalizacaoCompraService
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly ICompraRepository _compraRepository;
    private readonly IValidadorFinalizacaoCompra _validadorCheckout;
    private readonly IAcessoUtilizadorFactory _fabricaAcessoUtilizador;
    private readonly IGeradorReferenciaCompra _geradorReferenciaCompra;
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly IPagamentoService _pagamentoService;
    private readonly IEnumerable<ICompraObserver> _compraObservers;
    private readonly int _expiracaoMultibancoHoras;

    public FinalizacaoCompraService(
        ICarrinhoRepository carrinhoRepository,
        ICompraRepository compraRepository,
        IValidadorFinalizacaoCompra ValidadorFinalizacaoCompra,
        IAcessoUtilizadorFactory fabricaAcessoUtilizador,
        IGeradorReferenciaCompra geradorReferenciaCompra,
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        IPagamentoService pagamentoService,
        IEnumerable<ICompraObserver> compraObservers,
        IConfiguration configuration
    )
    {
        _carrinhoRepository = carrinhoRepository;
        _compraRepository = compraRepository;
        _validadorCheckout = ValidadorFinalizacaoCompra;
        _fabricaAcessoUtilizador = fabricaAcessoUtilizador;
        _geradorReferenciaCompra = geradorReferenciaCompra;
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _pagamentoService = pagamentoService;
        _compraObservers = compraObservers;
        _expiracaoMultibancoHoras = PagamentosConfiguracao.ObterExpiracaoMultibancoHoras(
            configuration
        );
    }

    public async Task<ResultadoFinalizacaoCompraDTO> FinalizarCompraAsync(
        int utilizadorId,
        string metodoPagamento = MetodosPagamento.CartaoCredito
    )
    {
        return await _compraRepository.ExecuteInTransactionAsync(async () =>
        {
            var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

            await _validadorCheckout.ValidarAsync(utilizadorId, carrinho);
            var carrinhoValido = carrinho!;

            var agora = DateTime.UtcNow;

            var compra = CriarCompra(utilizadorId, carrinhoValido, agora);

            compra.Pagamento = await _pagamentoService.ProcessarPagamentoSimuladoAsync(
                compra,
                agora,
                metodoPagamento
            );

            await _compraRepository.AddAsync(compra);

            var pagamentoAprovado = compra.Pagamento.Estado == EstadoPagamento.Aprovado;
            compra.Estado = pagamentoAprovado ? EstadoCompra.Pago : EstadoCompra.Pendente;
            compra.PagaEm = pagamentoAprovado ? agora : null;

            var acessosComprados = pagamentoAprovado
                ? carrinhoValido
                    .Itens.Select(item =>
                        _fabricaAcessoUtilizador.Criar(utilizadorId, compra, item, agora)
                    )
                    .ToList()
                : new List<AcessoUtilizador>();

            if (acessosComprados.Count > 0)
                await _acessoUtilizadorRepository.AddRangeAsync(acessosComprados);

            if (pagamentoAprovado)
            {
                var acessos = carrinhoValido.Itens.Select(item => item.Acesso).ToList();
                await Task.WhenAll(
                    _compraObservers.Select(observer =>
                        observer.NotificarAsync(utilizadorId, compra.ValorTotal, acessos)
                    )
                );
            }

            _carrinhoRepository.RemoveItems(carrinhoValido.Itens.ToList());

            carrinhoValido.AtualizadoEm = agora;

            await _compraRepository.SaveChangesAsync();

            var compraCriada = await _compraRepository.ObterPorIdAsync(compra.Id);

            return CompraMapper.MapToCheckoutResultadoDTO(
                compraCriada!,
                acessosComprados.Count,
                pagamentoAprovado
                    ? "Compra finalizada com sucesso."
                    : $"Referencia Multibanco gerada. O pagamento fica pendente durante {_expiracaoMultibancoHoras} horas.",
                _expiracaoMultibancoHoras
            );
        });
    }

    private Compra CriarCompra(int utilizadorId, Carrinho carrinho, DateTime dataCompra)
    {
        var compra = new Compra
        {
            UtilizadorId = utilizadorId,
            Referencia = _geradorReferenciaCompra.Gerar(),
            CriadaEm = dataCompra,
            Estado = EstadoCompra.Pendente,
            Itens = carrinho
                .Itens.Select(item => new ItemCompra
                {
                    AcessoId = item.AcessoId,
                    NomeAcesso = item.Acesso.Nome,
                    TipoAcesso = item.Acesso.Tipo,
                    PrecoUnitario = item.PrecoUnitario,
                    Quantidade = item.Quantidade,
                    Subtotal = item.PrecoUnitario * item.Quantidade,
                })
                .ToList(),
        };

        compra.ValorTotal = compra.Itens.Sum(i => i.Subtotal);

        return compra;
    }
}
