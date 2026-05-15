using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CheckoutService : ICheckoutService
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly ICompraRepository _compraRepository;
    private readonly IValidadorCheckout _validadorCheckout;
    private readonly IAcessoUtilizadorFactory _fabricaAcessoUtilizador;
    private readonly IGeradorReferenciaCompra _geradorReferenciaCompra;
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly IPagamentoService _pagamentoService;

    public CheckoutService(
        ICarrinhoRepository carrinhoRepository,
        ICompraRepository compraRepository,
        IValidadorCheckout validadorCheckout,
        IAcessoUtilizadorFactory fabricaAcessoUtilizador,
        IGeradorReferenciaCompra geradorReferenciaCompra,
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        IPagamentoService pagamentoService
    )
    {
        _carrinhoRepository = carrinhoRepository;
        _compraRepository = compraRepository;
        _validadorCheckout = validadorCheckout;
        _fabricaAcessoUtilizador = fabricaAcessoUtilizador;
        _geradorReferenciaCompra = geradorReferenciaCompra;
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _pagamentoService = pagamentoService;
    }

    public async Task<CheckoutResultadoDto> FinalizarCompraAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.GetByUtilizadorIdAsync(utilizadorId);

        await _validadorCheckout.ValidarAsync(utilizadorId, carrinho);

        var agora = DateTime.UtcNow;

        var compra = CriarCompra(utilizadorId, carrinho!, agora);

        compra.Pagamento = await _pagamentoService.ProcessarPagamentoSimuladoAsync(compra, agora);

        await _compraRepository.AddAsync(compra);

        var acessosComprados = carrinho!
            .Itens.Select(item => _fabricaAcessoUtilizador.Criar(utilizadorId, compra, item, agora))
            .ToList();

        await _acessoUtilizadorRepository.AddRangeAsync(acessosComprados);

        _carrinhoRepository.RemoveItems(carrinho.Itens.ToList());

        carrinho.AtualizadoEm = agora;

        await _compraRepository.SaveChangesAsync();

        var compraCriada = await _compraRepository.GetByIdAsync(compra.Id);

        return CompraMapper.MapToCheckoutResultadoDto(
            compraCriada!,
            acessosComprados.Count,
            "Compra finalizada com sucesso."
        );
    }

    private Compra CriarCompra(int utilizadorId, Carrinho carrinho, DateTime dataCompra)
    {
        var compra = new Compra
        {
            UtilizadorId = utilizadorId,
            Referencia = _geradorReferenciaCompra.Gerar(),
            CriadaEm = dataCompra,
            Estado = EstadoCompra.Pago,
            PagaEm = dataCompra,
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
