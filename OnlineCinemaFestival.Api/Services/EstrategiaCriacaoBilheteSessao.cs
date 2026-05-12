using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class EstrategiaCriacaoBilheteSessao : IEstrategiaCriacaoAcessoUtilizador
{
    public TipoAcesso Tipo => TipoAcesso.BilheteSessao;

    public AcessoUtilizador Criar(
        int utilizadorId,
        Compra compra,
        ItemCarrinho item,
        DateTime dataCompra
    )
    {
        var acesso = item.Acesso;

        if (acesso.SessaoId == null || acesso.Sessao == null)
            throw new InvalidOperationException("Bilhete de sessão sem sessão associada.");

        return new AcessoUtilizador
        {
            UtilizadorId = utilizadorId,
            Compra = compra,
            AcessoId = acesso.Id,
            TipoAcesso = acesso.Tipo,
            SessaoId = acesso.SessaoId,
            FestivalId = acesso.Sessao.FestivalId,
            FilmeId = null,
            InicioValidade = acesso.Sessao.Inicio,
            FimValidade = acesso.Sessao.Fim,
            Ativo = true,
            CriadoEm = dataCompra,
        };
    }
}
