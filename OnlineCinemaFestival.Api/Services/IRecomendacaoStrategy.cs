using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IRecomendacaoStrategy
{
    IEnumerable<ResultadoRecomendacao> Recomendar(
        IEnumerable<Filme> filmes,
        Utilizador? utilizador
    );
}
