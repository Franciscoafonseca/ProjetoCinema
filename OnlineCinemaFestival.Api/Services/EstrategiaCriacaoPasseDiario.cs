using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class EstrategiaCriacaoPasseDiario : IEstrategiaCriacaoAcessoUtilizador
{
    public TipoAcesso Tipo => TipoAcesso.PasseDiario;

    public AcessoUtilizador Criar(
        int utilizadorId,
        Compra compra,
        ItemCarrinho item,
        DateTime dataCompra
    )
    {
        var acesso = item.Acesso;

        if (acesso.FestivalId == null)
            throw new InvalidOperationException("Passe diário sem festival associado.");

        if (acesso.DataAcesso == null)
            throw new InvalidOperationException("Passe diário sem data de acesso.");

        var inicio = acesso.DataAcesso.Value.Date;
        var fim = inicio.AddDays(1);

        return new AcessoUtilizador
        {
            UtilizadorId = utilizadorId,
            Compra = compra,
            AcessoId = acesso.Id,
            TipoAcesso = acesso.Tipo,
            SessaoId = null,
            FestivalId = acesso.FestivalId,
            FilmeId = null,
            InicioValidade = inicio,
            FimValidade = fim,
            Ativo = true,
            CriadoEm = dataCompra,
        };
    }
}
