namespace OnlineCinemaFestival.Api.Configuracao;

public static class PagamentosConfiguracao
{
    public static int ObterExpiracaoMultibancoHoras(IConfiguration configuration)
    {
        var expiracaoHoras = configuration.GetValue<int>("Pagamentos:Multibanco:ExpiracaoHoras");

        if (expiracaoHoras <= 0)
            throw new InvalidOperationException(
                "Pagamentos:Multibanco:ExpiracaoHoras deve ser maior que zero."
            );

        return expiracaoHoras;
    }
}
