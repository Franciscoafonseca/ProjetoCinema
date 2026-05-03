using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public static class AcessoStrategyFactory
{
    public static IAcessoStrategy GetStrategy(TipoAcesso tipo)
    {
        // O Switch escolhe a regra de negócio correta baseada no Enum do Acesso.cs
        return tipo switch
        {
            TipoAcesso.BilheteUnico => new BilheteUnicoStrategy(),
            TipoAcesso.PasseDiario => new PasseDiarioStrategy(),
            TipoAcesso.PasseCompleto => new PasseCompletoStrategy(),
            TipoAcesso.AluguerDigital => new AluguerDigitalStrategy(),
            _ => throw new ArgumentException("Tipo de acesso inválido para o sistema.")
        };
    }
}