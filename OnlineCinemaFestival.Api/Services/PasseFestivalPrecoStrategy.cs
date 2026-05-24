using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PasseFestivalPrecoStrategy : IPrecoStrategy
{
    private readonly IConfiguration _configuration;
    private readonly decimal _descontoFestival;
    private readonly int _validadePasseCompletoDias;

    public PasseFestivalPrecoStrategy(IConfiguration configuration)
    {
        _configuration = configuration;
        _descontoFestival = AcessosConfiguracao.ObterDescontoFestival(configuration);
        _validadePasseCompletoDias = AcessosConfiguracao.ObterValidadePasseCompletoDias(
            configuration
        );
    }

    public bool CanHandle(TipoAcesso tipo) =>
        tipo == TipoAcesso.PasseDiario || tipo == TipoAcesso.PasseCompleto;

    public decimal CalcularPreco(CompraItemDto item)
    {
        var chave = item.Tipo == TipoAcesso.PasseCompleto
            ? ChavesPrecosAcesso.PasseCompletoCompraAvulsa
            : ChavesPrecosAcesso.PasseDiarioCompraAvulsa;

        return AcessosConfiguracao.ObterPreco(_configuration, chave) * (1 - _descontoFestival);
    }

    public DateTime? CalcularValidade(CompraItemDto item)
    {
        return item.Tipo == TipoAcesso.PasseCompleto
            ? DateTime.UtcNow.AddDays(_validadePasseCompletoDias)
            : DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
    }
}
