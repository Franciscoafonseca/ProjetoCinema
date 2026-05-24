using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class RecomendacaoPorAvaliacaoStrategy : IRecomendacaoStrategy
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
                Media = f.Avaliacoes.Count == 0
                    ? (decimal)(f.AvaliacaoTmdb ?? 0)
                    : (decimal)f.Avaliacoes.Average(a => a.Pontuacao),
            })
            .Where(f => f.Media > 0)
            .Select(f => new ResultadoRecomendacao(
                f.Filme,
                f.Media,
                "Bem avaliado"
            ));
    }
}
