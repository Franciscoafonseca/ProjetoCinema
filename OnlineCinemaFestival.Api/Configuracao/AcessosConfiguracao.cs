namespace OnlineCinemaFestival.Api.Configuracao;

public static class AcessosConfiguracao
{
    public static int ObterQuantidadeMaximaCarrinho(IConfiguration configuration)
    {
        var quantidadeMaxima = configuration.GetValue<int>("Acessos:QuantidadeMaximaCarrinho");

        if (quantidadeMaxima <= 0)
            throw new InvalidOperationException(
                "Acessos:QuantidadeMaximaCarrinho deve ser maior que zero."
            );

        return quantidadeMaxima;
    }

    public static int ObterDuracaoAluguerDigitalHoras(IConfiguration configuration)
    {
        var duracaoHoras = configuration.GetValue<int>("Acessos:DuracaoAluguerDigitalHoras");

        if (duracaoHoras <= 0)
            throw new InvalidOperationException(
                "Acessos:DuracaoAluguerDigitalHoras deve ser maior que zero."
            );

        return duracaoHoras;
    }

    public static decimal ObterPreco(IConfiguration configuration, string chave)
    {
        var preco = configuration.GetValue<decimal>($"Acessos:Precos:{chave}");

        if (preco <= 0)
            throw new InvalidOperationException($"Acessos:Precos:{chave} deve ser maior que zero.");

        return preco;
    }

    public static decimal ObterDescontoFestival(IConfiguration configuration)
    {
        var desconto = configuration.GetValue<decimal>("Acessos:DescontoFestival");

        if (desconto is < 0 or >= 1)
            throw new InvalidOperationException("Acessos:DescontoFestival deve estar entre 0 e 1.");

        return desconto;
    }

    public static int ObterValidadePasseCompletoDias(IConfiguration configuration)
    {
        var validadeDias = configuration.GetValue<int>("Acessos:ValidadePasseCompletoDias");

        if (validadeDias <= 0)
            throw new InvalidOperationException(
                "Acessos:ValidadePasseCompletoDias deve ser maior que zero."
            );

        return validadeDias;
    }
}
