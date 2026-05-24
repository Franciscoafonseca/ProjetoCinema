using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class RecomendacaoService : IRecomendacaoService
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IEnumerable<IRecomendacaoStrategy> _strategies;

    public RecomendacaoService(
        IFilmeRepository filmeRepository,
        IUtilizadorRepository utilizadorRepository,
        IEnumerable<IRecomendacaoStrategy> strategies
    )
    {
        _filmeRepository = filmeRepository;
        _utilizadorRepository = utilizadorRepository;
        _strategies = strategies;
    }

    public async Task<List<FilmeRecomendadoDTO>> ObterRecomendacoesAsync(
        int utilizadorId,
        int quantidade = 12
    )
    {
        var filmes = (await _filmeRepository.ObterTodosAsync()).ToList();
        if (filmes.Count == 0)
            return new List<FilmeRecomendadoDTO>();

        var utilizador = await _utilizadorRepository.ObterComPerfilAsync(utilizadorId);

        var combinadas = _strategies
            .SelectMany(strategy => strategy.Recomendar(filmes, utilizador))
            .GroupBy(r => r.Filme.Id)
            .Select(g => new
            {
                Filme = g.First().Filme,
                Pontuacao = g.Sum(r => r.Pontuacao),
                Motivos = g.Select(r => r.Motivo).Distinct().ToList(),
            })
            .OrderByDescending(r => r.Pontuacao)
            .ThenBy(r => r.Filme.Titulo)
            .Take(Math.Clamp(quantidade, 1, 50))
            .ToList();

        if (combinadas.Count == 0)
        {
            combinadas = filmes
                .OrderByDescending(f => f.Visualizacoes.Count)
                .ThenByDescending(f => f.Avaliacoes.Count == 0 ? f.AvaliacaoTmdb ?? 0 : f.Avaliacoes.Average(a => a.Pontuacao))
                .ThenBy(f => f.Titulo)
                .Take(Math.Clamp(quantidade, 1, 50))
                .Select(f => new
                {
                    Filme = f,
                    Pontuacao = 0m,
                    Motivos = new List<string> { "Sugestao geral do catalogo" },
                })
                .ToList();
        }

        return combinadas
            .Select(r => new FilmeRecomendadoDTO
            {
                Filme = FilmeMapper.MapToReadDTO(r.Filme),
                Pontuacao = r.Pontuacao,
                Motivo = string.Join("; ", r.Motivos),
            })
            .ToList();
    }
}
