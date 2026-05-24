namespace OnlineCinemaFestival.Api.Configuracao;

public sealed class PagamentoOptions
{
    public const string SectionName = "Pagamentos";

    public MultibancoOptions Multibanco { get; init; } = new();
}

public sealed class MultibancoOptions
{
    public string Entidade { get; init; } = string.Empty;

    public int ExpiracaoHoras { get; init; } = 3;
}
