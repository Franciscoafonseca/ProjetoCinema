using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IEstrategiaCriacaoAcessoUtilizador
{
    TipoAcesso Tipo { get; }

    AcessoUtilizador Criar(int utilizadorId, Compra compra, ItemCarrinho item, DateTime dataCompra);
}
