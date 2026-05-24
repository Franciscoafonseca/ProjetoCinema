using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    public sealed class FestivaisSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Festivais.Clear();
            contexto.Festivais.AddRange(await CriarFestivaisAsync(contexto.Db));

            await AssociarFilmesAFestivaisAsync(contexto.Db, contexto.Festivais, contexto.Filmes);
        }
    }

    private static async Task<List<Festival>> CriarFestivaisAsync(AppDbContext db)
    {
        var hoje = DateTime.UtcNow.Date;

        var festivaisSeed = new List<Festival>
        {
            new()
            {
                Name = "Festival Atlântico 2026",
                Description =
                    "Festival online dedicado ao cinema independente, europeu e atlântico. Está a decorrer neste momento.",
                StartDate = hoje.AddDays(-5),
                EndDate = hoje.AddDays(3),
            },
            new()
            {
                Name = "Madeira Indie Film Week",
                Description =
                    "Mostra de cinema independente com foco em novos realizadores. Festival atualmente ativo.",
                StartDate = hoje.AddDays(-2),
                EndDate = hoje.AddDays(5),
            },
            new()
            {
                Name = "Festival Sessões de Maio",
                Description =
                    "Festival especial com sessões recentes, sessões ao vivo e janela de acesso digital.",
                StartDate = hoje.AddDays(-1),
                EndDate = hoje.AddDays(6),
            },
            new()
            {
                Name = "Clássicos Online - Edição Passada",
                Description =
                    "Festival já terminado, útil para testar histórico e acessos expirados.",
                StartDate = hoje.AddDays(-35),
                EndDate = hoje.AddDays(-28),
            },
            new()
            {
                Name = "Curtas & Documentários Online",
                Description =
                    "Festival dedicado a curtas-metragens, documentários e cinema social.",
                StartDate = hoje.AddDays(7),
                EndDate = hoje.AddDays(13),
            },
            new()
            {
                Name = "Festival Sci-Fi Madeira",
                Description = "Festival de ficção científica, fantasia e cinema especulativo.",
                StartDate = hoje.AddDays(15),
                EndDate = hoje.AddDays(21),
            },
            new()
            {
                Name = "Noites de Terror Digital",
                Description = "Festival temático de terror, suspense e thrillers psicológicos.",
                StartDate = hoje.AddDays(28),
                EndDate = hoje.AddDays(34),
            },
            new()
            {
                Name = "Anima Madeira Online",
                Description = "Festival de animação, cinema juvenil e experiências visuais.",
                StartDate = hoje.AddDays(45),
                EndDate = hoje.AddDays(51),
            },
        };

        var existentes = await db.Festivals.ToListAsync();

        foreach (var festivalSeed in festivaisSeed)
        {
            var existente = existentes.FirstOrDefault(f => f.Name == festivalSeed.Name);

            if (existente == null)
            {
                await db.Festivals.AddAsync(festivalSeed);
            }
            else
            {
                existente.Description = festivalSeed.Description;
                existente.StartDate = festivalSeed.StartDate;
                existente.EndDate = festivalSeed.EndDate;
            }
        }

        await db.SaveChangesAsync();

        return await db.Festivals.ToListAsync();
    }

    private static async Task AssociarFilmesAFestivaisAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes
    )
    {
        var associacoesExistentes = await db.FestivalFilmes.ToListAsync();
        var novasAssociacoes = new List<FestivalFilme>();

        foreach (var festival in festivais)
        {
            var jaAssociados = associacoesExistentes
                .Where(ff => ff.FestivalId == festival.Id)
                .Select(ff => ff.FilmeId)
                .ToHashSet();

            foreach (
                var associacaoNova in novasAssociacoes.Where(ff => ff.FestivalId == festival.Id)
            )
                jaAssociados.Add(associacaoNova.FilmeId);

            var quantidadeAtual = jaAssociados.Count;
            var quantidadePretendida = SeedRandom.Next(18, 26);
            var quantidadeEmFalta = Math.Max(0, quantidadePretendida - quantidadeAtual);

            if (quantidadeEmFalta == 0)
                continue;

            var filmesParaAssociar = EscolherAleatorio(
                filmes.Where(f => !jaAssociados.Contains(f.Id)),
                quantidadeEmFalta
            );

            foreach (var filme in filmesParaAssociar)
            {
                novasAssociacoes.Add(
                    new FestivalFilme
                    {
                        FestivalId = festival.Id,
                        FilmeId = filme.Id,
                        ElegivelPremiosPublico = SeedRandom.Next(1, 100) <= 70,
                    }
                );
            }
        }

        if (novasAssociacoes.Count > 0)
        {
            await db.FestivalFilmes.AddRangeAsync(novasAssociacoes);
            await db.SaveChangesAsync();
        }

        foreach (var festival in festivais)
        {
            var associacoesFestival = await db
                .FestivalFilmes.Where(ff => ff.FestivalId == festival.Id)
                .OrderBy(ff => ff.FilmeId)
                .ToListAsync();

            if (
                associacoesFestival.Count > 0
                && !associacoesFestival.Any(ff => ff.ElegivelPremiosPublico)
            )
            {
                foreach (var associacao in associacoesFestival.Take(8))
                    associacao.ElegivelPremiosPublico = true;
            }
        }

        await db.SaveChangesAsync();
    }
}
