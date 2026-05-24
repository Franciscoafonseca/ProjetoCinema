using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public static partial class DbSeeder
{
    public sealed class FilmesSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Generos.Clear();
            contexto.Generos.AddRange(await CriarGenerosAsync(contexto.Db));

            contexto.Filmes.Clear();
            contexto.Filmes.AddRange(await CriarFilmesAsync(contexto.Db, contexto.Generos));
        }
    }

    private static async Task<List<Genero>> CriarGenerosAsync(AppDbContext db)
    {
        var nomesGeneros = new[]
        {
            "Drama",
            "Comédia",
            "Terror",
            "Documentário",
            "Animação",
            "Ação",
            "Ficção Científica",
            "Fantasia",
            "Romance",
            "Thriller",
            "Mistério",
            "Biografia",
            "Experimental",
            "Cinema Português",
        };

        var existentes = await db.Generos.ToListAsync();
        var nomesExistentes = existentes.Select(g => g.Name).ToHashSet();

        var novos = nomesGeneros
            .Where(nome => !nomesExistentes.Contains(nome))
            .Select(nome => new Genero { Name = nome, CreatedAt = DateTime.UtcNow })
            .ToList();

        if (novos.Count > 0)
        {
            await db.Generos.AddRangeAsync(novos);
            await db.SaveChangesAsync();
        }

        return await db.Generos.ToListAsync();
    }

    private static async Task<List<Filme>> CriarFilmesAsync(AppDbContext db, List<Genero> generos)
    {
        var filmesExistentes = await db.Filmes.ToListAsync();
        var filmes = new List<Filme>();

        foreach (var tmdb in FilmesTmdbSeed.Take(NumeroFilmes))
        {
            var filmeExistente =
                filmesExistentes.FirstOrDefault(f => f.TmdbId == tmdb.TmdbId)
                ?? filmesExistentes.FirstOrDefault(f =>
                    f.TmdbId >= 100000 && !FilmesTmdbSeed.Any(seed => seed.TmdbId == f.TmdbId)
                );

            if (filmeExistente == null)
            {
                filmeExistente = new Filme();
                filmes.Add(filmeExistente);
                filmesExistentes.Add(filmeExistente);
            }

            AplicarFilmeTmdbSeed(filmeExistente, tmdb);
        }

        if (filmes.Count > 0)
        {
            await db.Filmes.AddRangeAsync(filmes);
        }

        await db.SaveChangesAsync();

        return await db.Filmes.ToListAsync();
    }

    private static void AplicarFilmeTmdbSeed(Filme filme, FilmeTmdbSeed tmdb)
    {
        filme.TmdbId = tmdb.TmdbId;
        filme.Titulo = tmdb.Titulo;
        filme.TituloOriginal = tmdb.TituloOriginal;
        filme.Sinopse =
            $"Filme importado do conjunto de seed TMDB para demonstracao do catalogo: {tmdb.Titulo}.";
        filme.DataLancamento = DateTime.Parse(tmdb.DataLancamento, CultureInfo.InvariantCulture);
        filme.DuracaoMinutos = tmdb.DuracaoMinutos;
        filme.Genero = tmdb.Genero;
        filme.Classificacao = tmdb.AvaliacaoTmdb.ToString("0.0", CultureInfo.InvariantCulture);
        filme.AvaliacaoTmdb = tmdb.AvaliacaoTmdb;
        filme.CapaUrl = $"https://image.tmdb.org/t/p/w500{tmdb.CapaPath}";
        filme.Realizador = tmdb.Realizador;
        filme.TrailerUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ";
        filme.VideoProvider = "YouTube";
        filme.VideoKey = "dQw4w9WgXcQ";
        filme.VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ";
    }
}
