using Bogus;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Utilizadores.AnyAsync() || await db.Filmes.AnyAsync())
            return;

        Randomizer.Seed = new Random(2120622);

        var generos = new List<Genero>
        {
            new() { Name = "Drama", CreatedAt = DateTime.UtcNow },
            new() { Name = "Comédia", CreatedAt = DateTime.UtcNow },
            new() { Name = "Terror", CreatedAt = DateTime.UtcNow },
            new() { Name = "Documentário", CreatedAt = DateTime.UtcNow },
            new() { Name = "Animação", CreatedAt = DateTime.UtcNow },
            new() { Name = "Ação", CreatedAt = DateTime.UtcNow },
            new() { Name = "Ficção Científica", CreatedAt = DateTime.UtcNow },
        };

        await db.Generos.AddRangeAsync(generos);

        var festivalFaker = new Faker<Festival>("pt_PT")
            .RuleFor(f => f.Name, f => $"Festival {f.Address.City()} {f.Date.Future().Year}")
            .RuleFor(f => f.Description, f => f.Lorem.Paragraph())
            .RuleFor(f => f.StartDate, f => f.Date.Soon(30).Date)
            .RuleFor(
                f => f.EndDate,
                (faker, festival) => festival.StartDate.AddDays(faker.Random.Int(3, 10))
            );

        var festivals = festivalFaker.Generate(4);
        await db.Festivals.AddRangeAsync(festivals);

        var filmeFaker = new Faker<Filme>("pt_PT")
            .RuleFor(f => f.TmdbId, f => f.Random.Int(1000, 999999))
            .RuleFor(f => f.Titulo, f => f.Commerce.ProductName())
            .RuleFor(f => f.Sinopse, f => f.Lorem.Paragraph(2))
            .RuleFor(f => f.DataLancamento, f => f.Date.Past(20))
            .RuleFor(f => f.Genero, f => f.PickRandom(generos).Name)
            .RuleFor(f => f.Classificacao, f => f.Random.Decimal(5.0m, 9.8m).ToString("0.0"))
            .RuleFor(f => f.CapaUrl, f => $"https://picsum.photos/seed/{f.Random.Guid()}/400/600");

        var filmes = filmeFaker.Generate(30);
        await db.Filmes.AddRangeAsync(filmes);

        var utilizadorFaker = new Faker<Utilizador>("pt_PT")
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => "HASH_TESTE_123")
            .RuleFor(u => u.Role, f => default)
            .RuleFor(u => u.IsActive, f => true)
            .RuleFor(u => u.Nationality, f => "Portugal")
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(2).ToUniversalTime());

        var utilizadores = utilizadorFaker.Generate(8);
        await db.Utilizadores.AddRangeAsync(utilizadores);

        await db.SaveChangesAsync();

        var perfis = utilizadores
            .Select(u => new PerfilUtilizador
            {
                UtilizadorId = u.Id,
                Bio = "Perfil gerado automaticamente para testes.",
                ProfileImageUrl = $"https://picsum.photos/seed/perfil-{u.Id}/200/200",
                Location = "Madeira",
                IsPublic = true,
                CreatedAt = DateTime.UtcNow,
            })
            .ToList();

        await db.PerfisUtilizador.AddRangeAsync(perfis);

        var favoritos = new List<UtilizadorGeneroFavorito>();

        foreach (var utilizador in utilizadores)
        {
            foreach (var genero in generos.OrderBy(_ => Guid.NewGuid()).Take(3))
            {
                favoritos.Add(
                    new UtilizadorGeneroFavorito
                    {
                        UtilizadorId = utilizador.Id,
                        GeneroId = genero.Id,
                        CreatedAt = DateTime.UtcNow,
                    }
                );
            }
        }

        await db.UtilizadoresGenerosFavoritos.AddRangeAsync(favoritos);

        var comunidadeFaker = new Faker<Comunidade>("pt_PT")
            .RuleFor(
                c => c.Name,
                f =>
                    $"Comunidade {f.PickRandom("Cinema", "Terror", "Drama", "Clássicos", "Sci-Fi")}"
            )
            .RuleFor(c => c.Description, f => f.Lorem.Sentence())
            .RuleFor(
                c => c.ImageUrl,
                f => $"https://picsum.photos/seed/comunidade-{f.Random.Guid()}/600/300"
            )
            .RuleFor(c => c.IsPublic, f => true)
            .RuleFor(c => c.CreatedByUserId, f => f.PickRandom(utilizadores).Id)
            .RuleFor(c => c.CreatedAt, f => f.Date.Recent(20).ToUniversalTime());

        var comunidades = comunidadeFaker.Generate(5);
        await db.Comunidades.AddRangeAsync(comunidades);

        await db.SaveChangesAsync();

        var membros = new List<ComunidadeMembro>();

        foreach (var comunidade in comunidades)
        {
            foreach (var utilizador in utilizadores.OrderBy(_ => Guid.NewGuid()).Take(4))
            {
                membros.Add(
                    new ComunidadeMembro
                    {
                        ComunidadeId = comunidade.Id,
                        UtilizadorId = utilizador.Id,
                        Role = default,
                        JoinedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                    }
                );
            }
        }

        await db.ComunidadeMembros.AddRangeAsync(membros);

        var comentarioFaker = new Faker<Comentario>("pt_PT")
            .RuleFor(c => c.UsuarioId, f => f.PickRandom(utilizadores).Id)
            .RuleFor(c => c.ComunidadeId, f => f.PickRandom(comunidades).Id)
            .RuleFor(c => c.Texto, f => f.Lorem.Paragraph())
            .RuleFor(c => c.CriadoEm, f => f.Date.Recent(15).ToUniversalTime())
            .RuleFor(c => c.Visivel, f => true)
            .RuleFor(c => c.Reportado, f => false);

        var comentarios = comentarioFaker.Generate(25);
        await db.Comentarios.AddRangeAsync(comentarios);

        var avaliacoes = new List<Avaliacao>();

        foreach (var utilizador in utilizadores)
        {
            var filmesEscolhidos = filmes.OrderBy(_ => Guid.NewGuid()).Take(5);

            foreach (var filme in filmesEscolhidos)
            {
                avaliacoes.Add(
                    new Avaliacao
                    {
                        UsuarioId = utilizador.Id,
                        FilmeId = filme.Id,
                        Pontuacao = Random.Shared.Next(1, 11),
                        Data = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 20)),
                    }
                );
            }
        }

        await db.Avaliacoes.AddRangeAsync(avaliacoes);

        var listas = new List<ListaPessoal>();

        foreach (var utilizador in utilizadores)
        {
            listas.Add(
                new ListaPessoal
                {
                    UtilizadorId = utilizador.Id,
                    Name = "Quero ver",
                    Description = "Filmes que quero ver mais tarde.",
                    Tipo = default,
                    IsPublic = true,
                    CreatedAt = DateTime.UtcNow,
                }
            );

            listas.Add(
                new ListaPessoal
                {
                    UtilizadorId = utilizador.Id,
                    Name = "Favoritos",
                    Description = "Os meus filmes favoritos.",
                    Tipo = default,
                    IsPublic = true,
                    CreatedAt = DateTime.UtcNow,
                }
            );
        }

        await db.ListasPessoais.AddRangeAsync(listas);
        await db.SaveChangesAsync();

        var listaItems = new List<ListaPessoalItem>();

        foreach (var lista in listas)
        {
            var filmesDaLista = filmes.OrderBy(_ => Guid.NewGuid()).Take(5);

            foreach (var filme in filmesDaLista)
            {
                listaItems.Add(
                    new ListaPessoalItem
                    {
                        ListaPessoalId = lista.Id,
                        FilmeId = filme.Id,
                        AddedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 15)),
                    }
                );
            }
        }

        await db.ListaPessoalItems.AddRangeAsync(listaItems);

        await db.SaveChangesAsync();
    }
}
