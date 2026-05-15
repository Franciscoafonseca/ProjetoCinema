using Bogus;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        IPasswordHashingStrategy passwordHashingStrategy
    )
    {
        await db.Database.MigrateAsync();

        Randomizer.Seed = new Random(2120622);

        var agora = DateTime.UtcNow;

        var admin = await CriarAdminAsync(db, passwordHashingStrategy);
        var utilizadores = await CriarUtilizadoresAsync(db, passwordHashingStrategy);
        var todosUtilizadores = utilizadores.Append(admin).ToList();

        var generos = await CriarGenerosAsync(db);
        var festivais = await CriarFestivaisAsync(db);
        var filmes = await CriarFilmesAsync(db, generos);

        await AssociarFilmesAFestivaisAsync(db, festivais, filmes);

        var sessoes = await CriarSessoesAsync(db, festivais, filmes);

        var acessos = await CriarAcessosAsync(db, festivais, filmes, sessoes);

        await CriarPerfisAsync(db, todosUtilizadores);
        await CriarGenerosFavoritosAsync(db, utilizadores, generos);

        var comunidades = await CriarComunidadesAsync(db, utilizadores);
        await CriarMembrosComunidadesAsync(db, comunidades, utilizadores);
        await CriarComentariosAsync(db, comunidades, utilizadores);

        await CriarAvaliacoesAsync(db, utilizadores, filmes);
        await CriarListasPessoaisAsync(db, utilizadores, filmes);

        await CriarCarrinhosAsync(db, utilizadores, acessos);
        await CriarComprasEAcessosUtilizadorAsync(
            db,
            utilizadores,
            acessos,
            sessoes,
            festivais,
            filmes
        );
        await CriarVisualizacoesAsync(db, utilizadores, filmes);

        await db.SaveChangesAsync();
    }

    private static async Task<Utilizador> CriarAdminAsync(
        AppDbContext db,
        IPasswordHashingStrategy passwordHashingStrategy
    )
    {
        const string emailAdmin = "admin@festival.pt";
        const string passwordAdmin = "Admin123!";

        var admin = await db.Utilizadores.FirstOrDefaultAsync(u => u.Email == emailAdmin);

        if (admin != null)
            return admin;

        admin = new Utilizador
        {
            Name = "Administrador",
            Email = emailAdmin,
            Role = UserRole.Admin,
            IsActive = true,
            Nationality = "Portugal",
            CreatedAt = DateTime.UtcNow,
        };

        admin.PasswordHash = passwordHashingStrategy.HashPassword(admin, passwordAdmin);

        await db.Utilizadores.AddAsync(admin);
        await db.SaveChangesAsync();

        return admin;
    }

    private static async Task<List<Utilizador>> CriarUtilizadoresAsync(
        AppDbContext db,
        IPasswordHashingStrategy passwordHashingStrategy
    )
    {
        var utilizadoresExistentes = await db
            .Utilizadores.Where(u => u.Email.EndsWith("@teste.pt"))
            .ToListAsync();

        if (utilizadoresExistentes.Count >= 8)
            return utilizadoresExistentes;

        var utilizadores = new List<Utilizador>();

        for (var i = 1; i <= 8; i++)
        {
            var email = $"utilizador{i}@teste.pt";

            if (await db.Utilizadores.AnyAsync(u => u.Email == email))
                continue;

            var utilizador = new Utilizador
            {
                Name = $"Utilizador Teste {i}",
                Email = email,
                Role = UserRole.User,
                IsActive = true,
                Nationality = "Portugal",
                CreatedAt = DateTime.UtcNow.AddDays(-i),
            };

            utilizador.PasswordHash = passwordHashingStrategy.HashPassword(utilizador, "User123!");

            utilizadores.Add(utilizador);
        }

        await db.Utilizadores.AddRangeAsync(utilizadores);
        await db.SaveChangesAsync();

        return await db.Utilizadores.Where(u => u.Email.EndsWith("@teste.pt")).ToListAsync();
    }

    private static async Task<List<Genero>> CriarGenerosAsync(AppDbContext db)
    {
        if (await db.Generos.AnyAsync())
            return await db.Generos.ToListAsync();

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
        await db.SaveChangesAsync();

        return generos;
    }

    private static async Task<List<Festival>> CriarFestivaisAsync(AppDbContext db)
    {
        if (await db.Festivals.AnyAsync())
            return await db.Festivals.ToListAsync();

        var hoje = DateTime.UtcNow.Date;

        var festivais = new List<Festival>
        {
            new()
            {
                Name = "Festival Atlântico 2026",
                Description = "Festival online dedicado a cinema independente.",
                StartDate = hoje.AddDays(10),
                EndDate = hoje.AddDays(16),
            },
            new()
            {
                Name = "Festival Sci-Fi Madeira",
                Description = "Festival de ficção científica e fantasia.",
                StartDate = hoje.AddDays(20),
                EndDate = hoje.AddDays(26),
            },
            new()
            {
                Name = "Festival Curtas e Documentários",
                Description = "Festival focado em curtas-metragens e documentários.",
                StartDate = hoje.AddDays(30),
                EndDate = hoje.AddDays(35),
            },
        };

        await db.Festivals.AddRangeAsync(festivais);
        await db.SaveChangesAsync();

        return festivais;
    }

    private static async Task<List<Filme>> CriarFilmesAsync(AppDbContext db, List<Genero> generos)
    {
        if (await db.Filmes.AnyAsync())
            return await db.Filmes.ToListAsync();

        var filmeFaker = new Faker<Filme>("pt_PT")
            .RuleFor(f => f.TmdbId, f => f.Random.Int(1000, 999999))
            .RuleFor(f => f.Titulo, f => f.Commerce.ProductName())
            .RuleFor(f => f.Sinopse, f => f.Lorem.Paragraph(2))
            .RuleFor(f => f.DataLancamento, f => f.Date.Past(20).ToUniversalTime())
            .RuleFor(f => f.Genero, f => f.PickRandom(generos).Name)
            .RuleFor(f => f.Classificacao, f => f.Random.Decimal(5.0m, 9.8m).ToString("0.0"))
            .RuleFor(f => f.CapaUrl, f => $"https://picsum.photos/seed/{f.Random.Guid()}/400/600")
            .RuleFor(f => f.VideoUrl, _ => "https://www.youtube.com/embed/dQw4w9WgXcQ");

        var filmes = filmeFaker.Generate(30);

        await db.Filmes.AddRangeAsync(filmes);
        await db.SaveChangesAsync();

        return filmes;
    }

    private static async Task AssociarFilmesAFestivaisAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes
    )
    {
        if (await db.FestivalFilmes.AnyAsync())
            return;

        var associacoes = new List<FestivalFilme>();

        foreach (var festival in festivais)
        {
            var filmesDoFestival = filmes.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();

            foreach (var filme in filmesDoFestival)
            {
                associacoes.Add(new FestivalFilme { FestivalId = festival.Id, FilmeId = filme.Id });
            }
        }

        await db.FestivalFilmes.AddRangeAsync(associacoes);
        await db.SaveChangesAsync();
    }

    private static async Task<List<Sessao>> CriarSessoesAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes
    )
    {
        if (await db.Sessoes.AnyAsync())
            return await db.Sessoes.Include(s => s.FilmesDaSessao).ToListAsync();

        var sessoes = new List<Sessao>();

        foreach (var festival in festivais)
        {
            var filmeIdsDoFestival = await db
                .FestivalFilmes.Where(ff => ff.FestivalId == festival.Id)
                .Select(ff => ff.FilmeId)
                .ToListAsync();

            var filmesDoFestival = filmes
                .Where(f => filmeIdsDoFestival.Contains(f.Id))
                .Take(6)
                .ToList();

            var sessaoPrincipal = new Sessao
            {
                FestivalId = festival.Id,
                Tipo = TipoSessao.HorarioFixo,
                Inicio = festival.StartDate.Date.AddHours(20),
                Fim = festival.StartDate.Date.AddHours(23),
                TemChatAoVivo = true,
                Observacoes = "Sessão principal com chat ao vivo.",
            };

            var ordem = 1;

            foreach (var filme in filmesDoFestival.Take(5))
            {
                sessaoPrincipal.FilmesDaSessao.Add(
                    new SessaoFilme { FilmeId = filme.Id, Ordem = ordem++ }
                );
            }

            var sessaoRepeticao = new Sessao
            {
                FestivalId = festival.Id,
                Tipo = TipoSessao.HorarioFixo,
                Inicio = festival.StartDate.Date.AddDays(1).AddHours(20),
                Fim = festival.StartDate.Date.AddDays(1).AddHours(23),
                TemChatAoVivo = true,
                Observacoes = "Repetição da sessão principal.",
            };

            ordem = 1;

            foreach (var filme in filmesDoFestival.Take(5))
            {
                sessaoRepeticao.FilmesDaSessao.Add(
                    new SessaoFilme { FilmeId = filme.Id, Ordem = ordem++ }
                );
            }

            sessoes.Add(sessaoPrincipal);
            sessoes.Add(sessaoRepeticao);
        }

        await db.Sessoes.AddRangeAsync(sessoes);
        await db.SaveChangesAsync();

        return sessoes;
    }

    private static async Task<List<Acesso>> CriarAcessosAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes,
        List<Sessao> sessoes
    )
    {
        if (await db.Acessos.AnyAsync())
            return await db
                .Acessos.Include(a => a.Sessao)
                .Include(a => a.Festival)
                .Include(a => a.Filme)
                .ToListAsync();

        var acessos = new List<Acesso>();

        foreach (var sessao in sessoes)
        {
            acessos.Add(
                new Acesso
                {
                    Nome = $"Bilhete - Sessão {sessao.Id}",
                    Descricao = "Bilhete válido para uma sessão específica.",
                    Tipo = TipoAcesso.BilheteSessao,
                    Preco = 4.99m,
                    SessaoId = sessao.Id,
                    FestivalId = null,
                    FilmeId = null,
                    DataAcesso = null,
                    DuracaoHoras = null,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        foreach (var festival in festivais)
        {
            acessos.Add(
                new Acesso
                {
                    Nome = $"Passe Diário - {festival.Name}",
                    Descricao = "Passe válido para todas as sessões de um dia do festival.",
                    Tipo = TipoAcesso.PasseDiario,
                    Preco = 9.99m,
                    SessaoId = null,
                    FestivalId = festival.Id,
                    FilmeId = null,
                    DataAcesso = festival.StartDate.Date,
                    DuracaoHoras = null,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );

            acessos.Add(
                new Acesso
                {
                    Nome = $"Passe Completo - {festival.Name}",
                    Descricao = "Passe válido para todo o festival.",
                    Tipo = TipoAcesso.PasseCompleto,
                    Preco = 24.99m,
                    SessaoId = null,
                    FestivalId = festival.Id,
                    FilmeId = null,
                    DataAcesso = null,
                    DuracaoHoras = null,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        foreach (var filme in filmes.Take(10))
        {
            acessos.Add(
                new Acesso
                {
                    Nome = $"Aluguer Digital - {filme.Titulo}",
                    Descricao = "Aluguer individual do filme durante 48 horas.",
                    Tipo = TipoAcesso.AluguerDigital,
                    Preco = 3.99m,
                    SessaoId = null,
                    FestivalId = null,
                    FilmeId = filme.Id,
                    DataAcesso = null,
                    DuracaoHoras = 48,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        await db.Acessos.AddRangeAsync(acessos);
        await db.SaveChangesAsync();

        return acessos;
    }

    private static async Task CriarPerfisAsync(AppDbContext db, List<Utilizador> utilizadores)
    {
        foreach (var utilizador in utilizadores)
        {
            var jaTemPerfil = await db.PerfisUtilizador.AnyAsync(p =>
                p.UtilizadorId == utilizador.Id
            );

            if (jaTemPerfil)
                continue;

            await db.PerfisUtilizador.AddAsync(
                new PerfilUtilizador
                {
                    UtilizadorId = utilizador.Id,
                    Bio = "Perfil gerado automaticamente para testes.",
                    ProfileImageUrl = $"https://picsum.photos/seed/perfil-{utilizador.Id}/200/200",
                    Location = "Madeira",
                    IsPublic = true,
                    CreatedAt = DateTime.UtcNow,
                }
            );
        }

        await db.SaveChangesAsync();
    }

    private static async Task CriarGenerosFavoritosAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Genero> generos
    )
    {
        if (await db.UtilizadoresGenerosFavoritos.AnyAsync())
            return;

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
        await db.SaveChangesAsync();
    }

    private static async Task<List<Comunidade>> CriarComunidadesAsync(
        AppDbContext db,
        List<Utilizador> utilizadores
    )
    {
        if (await db.Comunidades.AnyAsync())
            return await db.Comunidades.ToListAsync();

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

        return comunidades;
    }

    private static async Task CriarMembrosComunidadesAsync(
        AppDbContext db,
        List<Comunidade> comunidades,
        List<Utilizador> utilizadores
    )
    {
        if (await db.ComunidadeMembros.AnyAsync())
            return;

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
        await db.SaveChangesAsync();
    }

    private static async Task CriarComentariosAsync(
        AppDbContext db,
        List<Comunidade> comunidades,
        List<Utilizador> utilizadores
    )
    {
        if (await db.Comentarios.AnyAsync())
            return;

        var comentarioFaker = new Faker<Comentario>("pt_PT")
            .RuleFor(c => c.UsuarioId, f => f.PickRandom(utilizadores).Id)
            .RuleFor(c => c.ComunidadeId, f => f.PickRandom(comunidades).Id)
            .RuleFor(c => c.Texto, f => f.Lorem.Paragraph())
            .RuleFor(c => c.CriadoEm, f => f.Date.Recent(15).ToUniversalTime())
            .RuleFor(c => c.Visivel, f => true)
            .RuleFor(c => c.Reportado, f => false);

        var comentarios = comentarioFaker.Generate(25);

        await db.Comentarios.AddRangeAsync(comentarios);
        await db.SaveChangesAsync();
    }

    private static async Task CriarAvaliacoesAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Filme> filmes
    )
    {
        if (await db.Avaliacoes.AnyAsync())
            return;

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
        await db.SaveChangesAsync();
    }

    private static async Task CriarListasPessoaisAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Filme> filmes
    )
    {
        if (await db.ListasPessoais.AnyAsync())
            return;

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

            listas.Add(
                new ListaPessoal
                {
                    UtilizadorId = utilizador.Id,
                    Name = "Vistos",
                    Description = "Filmes já visualizados.",
                    Tipo = default,
                    IsPublic = false,
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

    private static async Task CriarCarrinhosAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Acesso> acessos
    )
    {
        if (await db.Carrinhos.AnyAsync())
            return;

        var carrinhos = new List<Carrinho>();

        foreach (var utilizador in utilizadores.Take(3))
        {
            var carrinho = new Carrinho
            {
                UtilizadorId = utilizador.Id,
                DataCriacao = DateTime.UtcNow.AddDays(-1),
                AtualizadoEm = DateTime.UtcNow,
            };

            foreach (var acesso in acessos.OrderBy(_ => Guid.NewGuid()).Take(2))
            {
                carrinho.Itens.Add(
                    new CarrinhoItem
                    {
                        AcessoId = acesso.Id,
                        PrecoUnitario = acesso.Preco,
                        Quantidade = 1,
                        DataAdicao = DateTime.UtcNow.AddHours(-Random.Shared.Next(1, 12)),
                    }
                );
            }

            carrinhos.Add(carrinho);
        }

        await db.Carrinhos.AddRangeAsync(carrinhos);
        await db.SaveChangesAsync();
    }

    private static async Task CriarComprasEAcessosUtilizadorAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Acesso> acessos,
        List<Sessao> sessoes,
        List<Festival> festivais,
        List<Filme> filmes
    )
    {
        if (await db.Compras.AnyAsync())
            return;

        var compras = new List<Compra>();
        var acessosUtilizador = new List<AcessoUtilizador>();

        foreach (var utilizador in utilizadores.Take(4))
        {
            var acessosComprados = acessos.OrderBy(_ => Guid.NewGuid()).Take(2).ToList();

            var compra = new Compra
            {
                UtilizadorId = utilizador.Id,
                Referencia = $"CMP-SEED-{utilizador.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Estado = EstadoCompra.Pago,
                CriadaEm = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 5)),
                PagaEm = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 4)),
            };

            foreach (var acesso in acessosComprados)
            {
                compra.Itens.Add(
                    new ItemCompra
                    {
                        AcessoId = acesso.Id,
                        NomeAcesso = acesso.Nome,
                        TipoAcesso = acesso.Tipo,
                        PrecoUnitario = acesso.Preco,
                        Quantidade = 1,
                        Subtotal = acesso.Preco,
                    }
                );

                var validade = CalcularValidadeAcessoSeed(acesso);

                acessosUtilizador.Add(
                    new AcessoUtilizador
                    {
                        UtilizadorId = utilizador.Id,
                        AcessoId = acesso.Id,
                        Compra = compra,
                        TipoAcesso = acesso.Tipo,
                        SessaoId = acesso.SessaoId,
                        FestivalId = acesso.FestivalId ?? acesso.Sessao?.FestivalId,
                        FilmeId = acesso.FilmeId,
                        InicioValidade = validade.inicio,
                        FimValidade = validade.fim,
                        Ativo = true,
                        CriadoEm = DateTime.UtcNow,
                    }
                );
            }

            compra.ValorTotal = compra.Itens.Sum(i => i.Subtotal);
            compra.Pagamento = new Pagamento
            {
                Referencia = $"PG-{compra.Referencia}",
                Valor = compra.ValorTotal,
                Metodo = "Simulado",
                Estado = EstadoPagamento.Aprovado,
                CriadoEm = compra.PagaEm ?? compra.CriadaEm,
                ProcessadoEm = compra.PagaEm ?? compra.CriadaEm,
                Mensagem = "Pagamento seed aprovado automaticamente.",
            };

            compras.Add(compra);
        }

        await db.Compras.AddRangeAsync(compras);
        await db.AcessosUtilizador.AddRangeAsync(acessosUtilizador);

        await db.SaveChangesAsync();
    }

    private static (DateTime inicio, DateTime fim) CalcularValidadeAcessoSeed(Acesso acesso)
    {
        var agora = DateTime.UtcNow;

        return acesso.Tipo switch
        {
            TipoAcesso.BilheteSessao when acesso.Sessao != null => (
                acesso.Sessao.Inicio,
                acesso.Sessao.Fim
            ),

            TipoAcesso.PasseDiario when acesso.DataAcesso.HasValue => (
                acesso.DataAcesso.Value.Date,
                acesso.DataAcesso.Value.Date.AddDays(1)
            ),

            TipoAcesso.PasseCompleto when acesso.Festival != null => (
                acesso.Festival.StartDate,
                acesso.Festival.EndDate
            ),

            TipoAcesso.AluguerDigital => (
                agora.AddHours(-2),
                agora.AddHours(acesso.DuracaoHoras ?? 48)
            ),

            _ => (agora, agora.AddDays(1)),
        };
    }

    private static async Task CriarVisualizacoesAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Filme> filmes
    )
    {
        if (await db.Visualizacoes.AnyAsync())
            return;

        var visualizacoes = new List<Visualizacao>();

        foreach (var utilizador in utilizadores.Take(4))
        {
            foreach (var filme in filmes.OrderBy(_ => Guid.NewGuid()).Take(3))
            {
                visualizacoes.Add(
                    new Visualizacao
                    {
                        UtilizadorId = utilizador.Id,
                        FilmeId = filme.Id,
                        TipoConteudo = "Filme",
                        UrlVisualizacao =
                            filme.VideoUrl ?? "https://www.youtube.com/embed/dQw4w9WgXcQ",
                        VisualizadoEm = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 12)),
                    }
                );
            }
        }

        await db.Visualizacoes.AddRangeAsync(visualizacoes);
        await db.SaveChangesAsync();
    }
}
