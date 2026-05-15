using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class EstrategiaCriacaoAluguerDigital : IEstrategiaCriacaoAcessoUtilizador
{
    public TipoAcesso Tipo => TipoAcesso.AluguerDigital;

    public AcessoUtilizador Criar(
        int utilizadorId,
        Compra compra,
        CarrinhoItem item,
        DateTime dataCompra
    )
    {
        var acesso = item.Acesso;

        if (acesso.FilmeId == null)
            throw new InvalidOperationException("Aluguer digital sem filme associado.");

        var duracaoHoras = acesso.DuracaoHoras ?? 48;

        return new AcessoUtilizador
        {
            UtilizadorId = utilizadorId,
            Compra = compra,
            AcessoId = acesso.Id,
            TipoAcesso = acesso.Tipo,
            SessaoId = null,
            FestivalId = acesso.FestivalId,
            FilmeId = acesso.FilmeId,
            InicioValidade = dataCompra,
            FimValidade = dataCompra.AddHours(duracaoHoras),
            Ativo = true,
            CriadoEm = dataCompra,
        };
    }
}
