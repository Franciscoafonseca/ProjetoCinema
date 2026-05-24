namespace OnlineCinemaFestival.Api.Configuracao;

public sealed class AcessosOptions
{
    public const string SectionName = "Acessos";

    public int QuantidadeMaximaCarrinho { get; init; } = 99;

    public int DuracaoAluguerDigitalHoras { get; init; } = 48;

    public decimal DescontoFestival { get; init; } = 0.1m;

    public int ValidadePasseCompletoDias { get; init; } = 15;

    public Dictionary<string, decimal> Precos { get; init; } = [];
}
