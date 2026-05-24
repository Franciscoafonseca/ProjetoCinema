using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public static class ReviewListagemPolicy
{
    public const int QuantidadeInicial = 10;
    public const int Incremento = 10;

    public static IReadOnlyList<AvaliacaoDTO> ObterVisiveis(
        IEnumerable<AvaliacaoDTO> reviews,
        int quantidade
    )
    {
        return reviews
            .OrderByDescending(r => r.Data)
            .Take(Math.Max(0, quantidade))
            .ToList();
    }

    public static int ProximaQuantidade(int total, int atual)
    {
        return Math.Min(Math.Max(0, atual) + Incremento, Math.Max(0, total));
    }
}
