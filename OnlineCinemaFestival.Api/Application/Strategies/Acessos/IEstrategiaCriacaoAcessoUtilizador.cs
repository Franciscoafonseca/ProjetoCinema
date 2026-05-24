using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IEstrategiaCriacaoAcessoUtilizador
{
    TipoAcesso Tipo { get; }

    AcessoUtilizador Criar(int utilizadorId, Compra compra, CarrinhoItem item, DateTime dataCompra);
}
