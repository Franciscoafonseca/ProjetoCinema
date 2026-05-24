using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public static partial class DbSeeder
{
    public sealed class ReviewsSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            await CriarAvaliacoesAsync(contexto.Db, contexto.Utilizadores, contexto.Filmes);
            await CriarVisualizacoesAsync(contexto.Db, contexto.Utilizadores, contexto.Filmes);
        }
    }

    private static async Task CriarAvaliacoesAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Filme> filmes
    )
    {
        var existentes = await db.Avaliacoes.ToListAsync();

        if (existentes.Count >= 420)
            return;

        var chavesExistentes = existentes.Select(a => Chave(a.UsuarioId, a.FilmeId)).ToHashSet();

        var avaliacoes = new List<Avaliacao>();

        var filmesPopulares = filmes.OrderBy(f => f.Id).Take(18).ToList();

        foreach (var filme in filmesPopulares)
        {
            var utilizadoresParaFilme = EscolherAleatorio(utilizadores, SeedRandom.Next(15, 28));

            foreach (var utilizador in utilizadoresParaFilme)
            {
                var chave = Chave(utilizador.Id, filme.Id);

                if (chavesExistentes.Contains(chave))
                    continue;

                avaliacoes.Add(
                    new Avaliacao
                    {
                        UsuarioId = utilizador.Id,
                        FilmeId = filme.Id,
                        Pontuacao = SeedRandom.Next(7, 11),
                        Data = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 90)),
                    }
                );

                chavesExistentes.Add(chave);
            }
        }

        foreach (var utilizador in utilizadores)
        {
            var filmesEscolhidos = EscolherAleatorio(filmes, SeedRandom.Next(7, 16));

            foreach (var filme in filmesEscolhidos)
            {
                var chave = Chave(utilizador.Id, filme.Id);

                if (chavesExistentes.Contains(chave))
                    continue;

                avaliacoes.Add(
                    new Avaliacao
                    {
                        UsuarioId = utilizador.Id,
                        FilmeId = filme.Id,
                        Pontuacao = SeedRandom.Next(1, 11),
                        Data = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 120)),
                    }
                );

                chavesExistentes.Add(chave);
            }
        }

        if (avaliacoes.Count > 0)
        {
            await db.Avaliacoes.AddRangeAsync(avaliacoes);
            await db.SaveChangesAsync();
        }
    }

    private static async Task CriarVisualizacoesAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Filme> filmes
    )
    {
        var quantidadeExistente = await db.Visualizacoes.CountAsync();

        if (quantidadeExistente >= NumeroVisualizacoes)
            return;

        var visualizacoes = new List<Visualizacao>();
        var quantidadeEmFalta = NumeroVisualizacoes - quantidadeExistente;

        for (var i = 0; i < quantidadeEmFalta; i++)
        {
            var utilizador = EscolherAleatorio(utilizadores, 1).Single();
            var filme = EscolherAleatorio(filmes, 1).Single();

            visualizacoes.Add(
                new Visualizacao
                {
                    UtilizadorId = utilizador.Id,
                    FilmeId = filme.Id,
                    TipoConteudo = "Filme",
                    UrlVisualizacao = filme.VideoUrl ?? "https://www.youtube.com/embed/dQw4w9WgXcQ",
                    VisualizadoEm = DateTime
                        .UtcNow.AddDays(-SeedRandom.Next(0, 100))
                        .AddMinutes(-SeedRandom.Next(1, 900)),
                }
            );
        }

        await db.Visualizacoes.AddRangeAsync(visualizacoes);
        await db.SaveChangesAsync();
    }
}
