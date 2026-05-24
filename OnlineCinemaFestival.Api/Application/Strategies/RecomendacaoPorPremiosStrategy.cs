using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public class RecomendacaoPorPremiosStrategy : IRecomendacaoStrategy
{
    public IEnumerable<ResultadoRecomendacao> Recomendar(
        IEnumerable<Filme> filmes,
        Utilizador? utilizador
    )
    {
        return filmes
            .Select(f => new
            {
                Filme = f,
                Pontos = f.VotosPremiosFestival.Count + (f.ResultadosPremiosFestival.Count * 5),
            })
            .Where(f => f.Pontos > 0)
            .Select(f => new ResultadoRecomendacao(
                f.Filme,
                f.Pontos,
                f.Filme.ResultadosPremiosFestival.Count > 0
                    ? "Premiado em festivais"
                    : "Votado em premios"
            ));
    }
}
