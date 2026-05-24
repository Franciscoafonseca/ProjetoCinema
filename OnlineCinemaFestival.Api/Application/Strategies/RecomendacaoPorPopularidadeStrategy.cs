using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public class RecomendacaoPorPopularidadeStrategy : IRecomendacaoStrategy
{
    public IEnumerable<ResultadoRecomendacao> Recomendar(
        IEnumerable<Filme> filmes,
        Utilizador? utilizador
    )
    {
        return filmes
            .Where(f => f.Visualizacoes.Count > 0)
            .Select(f => new ResultadoRecomendacao(
                f,
                f.Visualizacoes.Count,
                "Popular entre espectadores"
            ));
    }
}
