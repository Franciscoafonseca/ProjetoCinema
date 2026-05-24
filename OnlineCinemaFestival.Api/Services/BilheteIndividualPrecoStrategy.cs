using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class BilheteIndividualPrecoStrategy : IPrecoStrategy
{
    private readonly IConfiguration _configuration;
    private readonly int _duracaoAluguerDigitalHoras;

    public BilheteIndividualPrecoStrategy(IConfiguration configuration)
    {
        _configuration = configuration;
        _duracaoAluguerDigitalHoras = AcessosConfiguracao.ObterDuracaoAluguerDigitalHoras(
            configuration
        );
    }

    public bool CanHandle(TipoAcesso tipo) =>
        tipo == TipoAcesso.BilheteSessao || tipo == TipoAcesso.AluguerDigital;

    public decimal CalcularPreco(CompraItemDto item)
    {
        return item.Tipo == TipoAcesso.AluguerDigital
            ? AcessosConfiguracao.ObterPreco(_configuration, ChavesPrecosAcesso.AluguerDigital)
            : AcessosConfiguracao.ObterPreco(_configuration, ChavesPrecosAcesso.BilheteIndividual);
    }

    public DateTime? CalcularValidade(CompraItemDto item)
    {
        return item.Tipo == TipoAcesso.AluguerDigital
            ? DateTime.UtcNow.AddHours(_duracaoAluguerDigitalHoras)
            : null;
    }
}
