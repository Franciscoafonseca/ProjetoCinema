using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Common.Errors;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public class EstrategiaCriacaoAluguerDigital : IEstrategiaCriacaoAcessoUtilizador
{
    private readonly int _duracaoAluguerDigitalHoras;

    public EstrategiaCriacaoAluguerDigital(IOptions<AcessosOptions> acessosOptions)
    {
        _duracaoAluguerDigitalHoras = acessosOptions.Value.DuracaoAluguerDigitalHoras;
    }

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
            throw new ConflitoDominioException("Aluguer digital sem filme associado.");

        var duracaoHoras = acesso.DuracaoHoras ?? _duracaoAluguerDigitalHoras;

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
