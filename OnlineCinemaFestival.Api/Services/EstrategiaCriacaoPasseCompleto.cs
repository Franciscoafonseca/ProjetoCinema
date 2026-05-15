using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class EstrategiaCriacaoPasseCompleto : IEstrategiaCriacaoAcessoUtilizador
{
    public TipoAcesso Tipo => TipoAcesso.PasseCompleto;

    public AcessoUtilizador Criar(
        int utilizadorId,
        Compra compra,
        CarrinhoItem item,
        DateTime dataCompra
    )
    {
        var acesso = item.Acesso;

        if (acesso.FestivalId == null || acesso.Festival == null)
            throw new InvalidOperationException("Passe completo sem festival associado.");

        return new AcessoUtilizador
        {
            UtilizadorId = utilizadorId,
            Compra = compra,
            AcessoId = acesso.Id,
            TipoAcesso = acesso.Tipo,
            SessaoId = null,
            FestivalId = acesso.FestivalId,
            FilmeId = null,
            InicioValidade = acesso.Festival.StartDate,
            FimValidade = acesso.Festival.EndDate,
            Ativo = true,
            CriadoEm = dataCompra,
        };
    }
}
