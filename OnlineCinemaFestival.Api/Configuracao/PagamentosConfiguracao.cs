namespace OnlineCinemaFestival.Api.Configuracao;

public static class PagamentosConfiguracao
{
    public static string ObterEntidadeMultibanco(IConfiguration configuration)
    {
        var entidade = configuration["Pagamentos:Multibanco:Entidade"];

        if (string.IsNullOrWhiteSpace(entidade))
            throw new InvalidOperationException(
                "Pagamentos:Multibanco:Entidade nao configurada na configuracao."
            );

        return entidade;
    }

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
