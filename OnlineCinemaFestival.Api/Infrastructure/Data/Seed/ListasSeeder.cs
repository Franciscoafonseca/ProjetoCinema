using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public static partial class DbSeeder
{
    public sealed class ListasSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            await CriarListasPessoaisAsync(contexto.Db, contexto.Utilizadores, contexto.Filmes);
        }
    }

    private static async Task CriarListasPessoaisAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Filme> filmes
    )
    {
        var listasExistentes = await db.ListasPessoais.ToListAsync();
        var listasNovas = new List<ListaPessoal>();

        foreach (var utilizador in utilizadores)
        {
            var listasDoUtilizador = listasExistentes
                .Where(l => l.UtilizadorId == utilizador.Id)
                .ToList();

            if (!listasDoUtilizador.Any(l => l.Name == "Quero ver"))
            {
                listasNovas.Add(
                    new ListaPessoal
                    {
                        UtilizadorId = utilizador.Id,
                        Name = "Quero ver",
                        Description = "Filmes que quero ver mais tarde.",
                        Tipo = TipoListaPessoal.Watchlist,
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 80)),
                    }
                );
            }

            if (!listasDoUtilizador.Any(l => l.Name == "Favoritos"))
            {
                listasNovas.Add(
                    new ListaPessoal
                    {
                        UtilizadorId = utilizador.Id,
                        Name = "Favoritos",
                        Description = "Os meus filmes favoritos.",
                        Tipo = TipoListaPessoal.Favorites,
                        IsPublic = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 80)),
                    }
                );
            }

            if (!listasDoUtilizador.Any(l => l.Name == "Vistos"))
            {
                listasNovas.Add(
                    new ListaPessoal
                    {
                        UtilizadorId = utilizador.Id,
                        Name = "Vistos",
                        Description = "Filmes já visualizados.",
                        Tipo = TipoListaPessoal.Watched,
                        IsPublic = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 80)),
                    }
                );
            }
        }

        if (listasNovas.Count > 0)
        {
            await db.ListasPessoais.AddRangeAsync(listasNovas);
            await db.SaveChangesAsync();
        }

        var listas = await db.ListasPessoais.ToListAsync();
        var itensExistentes = await db.ListaPessoalItems.ToListAsync();

        var chavesItensExistentes = itensExistentes
            .Select(i => Chave(i.ListaPessoalId, i.FilmeId))
            .ToHashSet();

        var itensNovos = new List<ListaPessoalItem>();

        foreach (var lista in listas)
        {
            var quantidadeAtual = itensExistentes.Count(i => i.ListaPessoalId == lista.Id);
            var quantidadePretendida = lista.Tipo switch
            {
                TipoListaPessoal.Favorites => SeedRandom.Next(6, 13),
                TipoListaPessoal.Watched => SeedRandom.Next(10, 22),
                TipoListaPessoal.Watchlist => SeedRandom.Next(8, 20),
                _ => SeedRandom.Next(5, 12),
            };

            var quantidadeEmFalta = Math.Max(0, quantidadePretendida - quantidadeAtual);

            if (quantidadeEmFalta == 0)
                continue;

            var filmesDaLista = EscolherAleatorio(filmes, quantidadeEmFalta + 5);

            foreach (var filme in filmesDaLista)
            {
                var chave = Chave(lista.Id, filme.Id);

                if (chavesItensExistentes.Contains(chave))
                    continue;

                itensNovos.Add(
                    new ListaPessoalItem
                    {
                        ListaPessoalId = lista.Id,
                        FilmeId = filme.Id,
                        AddedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 90)),
                    }
                );

                chavesItensExistentes.Add(chave);

                if (itensNovos.Count(i => i.ListaPessoalId == lista.Id) >= quantidadeEmFalta)
                    break;
            }
        }

        if (itensNovos.Count > 0)
        {
            await db.ListaPessoalItems.AddRangeAsync(itensNovos);
            await db.SaveChangesAsync();
        }
    }
}
