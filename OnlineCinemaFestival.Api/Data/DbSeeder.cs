using System.Globalization;
using Bogus;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Data;

public static class DbSeeder
{
    private const int NumeroUtilizadores = 35;
    private const int NumeroFilmes = 100;
    private const int NumeroComunidades = 10;
    private const int NumeroComentarios = 180;
    private const int NumeroCompras = 60;
    private const int NumeroVisualizacoes = 220;
    private const string MarcadorSessaoChatTeste = "CHAT_ATIVO_TESTE";

    private static readonly Random SeedRandom = new(2120622);

    private static readonly string[] Nacionalidades =
    {
        "Portugal",
        "Brasil",
        "Cabo Verde",
        "Angola",
        "Moçambique",
        "Espanha",
        "França",
        "Itália",
        "Reino Unido",
        "Alemanha",
    };

    private static readonly string[] Localizacoes =
    {
        "Funchal",
        "Câmara de Lobos",
        "Machico",
        "Santa Cruz",
        "Ribeira Brava",
        "Lisboa",
        "Porto",
        "Coimbra",
        "Braga",
        "Setúbal",
    };

    private static readonly string[] TitulosFilmesSeed =
    {
        "A Última Sessão no Funchal",
        "Maré Alta",
        "O Silêncio da Baía",
        "Depois da Meia-Noite",
        "Cartas para Atlântida",
        "A Casa das Sombras",
        "Neon Madeira",
        "O Arquivo das Estrelas",
        "O Homem que Filmava Nuvens",
        "Vozes do Mercado",
        "O Jardim Submerso",
        "A Ilha dos Esquecidos",
        "Noite de Estreia",
        "O Último Projetor",
        "Fronteira Digital",
        "A Rapariga e o Cometa",
        "O Peso da Luz",
        "Câmara Lenta",
        "O Som do Nevoeiro",
        "Pequenos Futuros",
        "A Cidade Sem Intervalo",
        "O Plano Sequência",
        "Fora de Campo",
        "O Festival Acabou Ontem",
        "A Máquina de Sonhar",
        "O Farol Vermelho",
        "A Teoria do Eclipse",
        "Dias de Cinema",
        "Fragmentos do Atlântico",
        "O Realizador Invisível",
        "As Ruas Depois da Chuva",
        "A Montanha Azul",
        "O Segredo da Sala 7",
        "Cineclube dos Fantasmas",
        "Uma Janela Para Marte",
        "A Balada do Último Bilhete",
        "O Som das Estrelas",
        "A Cidade Submersa",
        "Rostos no Escuro",
        "O Público Decide",
    };

    private static readonly string[] TextosComentarios =
    {
        "Gostei muito da fotografia e da atmosfera do filme.",
        "A sessão funcionou muito bem e o chat tornou a experiência mais interessante.",
        "A realização está acima da média e o ritmo prende bastante.",
        "Achei o início um pouco lento, mas o final compensa.",
        "Este filme merece estar nos prémios do público.",
        "A banda sonora ficou excelente e combinou muito bem com a narrativa.",
        "Boa escolha para este festival.",
        "Não esperava gostar tanto desta obra.",
        "O final ficou muito forte e deixou espaço para discussão.",
        "A personagem principal está muito bem construída.",
        "Gostei da forma como o filme explora o tema da memória.",
        "A sessão devia ter tido mais tempo para debate.",
        "A qualidade visual surpreendeu bastante.",
        "O filme é simples, mas muito eficaz.",
        "Gostava de ver mais filmes deste realizador na plataforma.",
    };

    private static readonly string[] NomesComunidades =
    {
        "Comunidade Cinema Atlântico",
        "Clube Sci-Fi Madeira",
        "Cinéfilos de Terror",
        "Curtas e Documentários",
        "Cinema Português e Lusófono",
        "Animação e Fantasia",
        "Festival Talks",
        "Críticos do Público",
        "Indie Lovers",
        "Sessões da Meia-Noite",
    };

    public static async Task SeedAsync(
        AppDbContext db,
        IPasswordHashingStrategy passwordHashingStrategy
    )
    {
        await db.Database.MigrateAsync();

        Randomizer.Seed = new Random(2120622);

        var admin = await CriarAdminAsync(db, passwordHashingStrategy);
        var utilizadores = await CriarUtilizadoresAsync(db, passwordHashingStrategy);
        var todosUtilizadores = utilizadores.Append(admin).ToList();

        var generos = await CriarGenerosAsync(db);
        var festivais = await CriarFestivaisAsync(db);
        var filmes = await CriarFilmesAsync(db, generos);

        await AssociarFilmesAFestivaisAsync(db, festivais, filmes);

        var sessoes = await CriarSessoesAsync(db, festivais, filmes);
        var sessaoChatTeste = await CriarSessaoChatAoVivoTesteAsync(db, festivais, filmes);

        sessoes = await db.Sessoes.Include(s => s.FilmesDaSessao).ToListAsync();

        var acessos = await CriarAcessosAsync(db, festivais, filmes, sessoes);

        await CriarPerfisAsync(db, todosUtilizadores);
        await CriarGenerosFavoritosAsync(db, utilizadores, generos);

        var comunidades = await CriarComunidadesAsync(db, utilizadores);
        await CriarMembrosComunidadesAsync(db, comunidades, utilizadores);
        await CriarComentariosAsync(db, comunidades, utilizadores);

        await CriarAvaliacoesAsync(db, utilizadores, filmes);
        await CriarListasPessoaisAsync(db, utilizadores, filmes);

        await CriarCarrinhosAsync(db, utilizadores, acessos);
        await CriarComprasEAcessosUtilizadorAsync(db, utilizadores, acessos);
        await GarantirAcessoSessaoChatTesteAsync(db, utilizadores, sessaoChatTeste);
        await CriarVisualizacoesAsync(db, utilizadores, filmes);

        await db.SaveChangesAsync();
    }

    private static List<T> EscolherAleatorio<T>(IEnumerable<T> origem, int quantidade)
    {
        var lista = origem.ToList();

        if (lista.Count == 0 || quantidade <= 0)
            return new List<T>();

        return lista
            .OrderBy(_ => SeedRandom.Next())
            .Take(Math.Min(quantidade, lista.Count))
            .ToList();
    }

    private static string Chave(params object?[] valores)
    {
        return string.Join(":", valores.Select(v => v?.ToString() ?? ""));
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
            Role = PapelUtilizador.Administrador,
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
        var faker = new Faker("pt_PT");

        var utilizadoresExistentes = await db
            .Utilizadores.Where(u => u.Email.EndsWith("@teste.pt"))
            .ToListAsync();

        if (utilizadoresExistentes.Count >= NumeroUtilizadores)
            return utilizadoresExistentes;

        var utilizadores = new List<Utilizador>();

        for (var i = 1; i <= NumeroUtilizadores; i++)
        {
            var email = $"utilizador{i}@teste.pt";

            if (await db.Utilizadores.AnyAsync(u => u.Email == email))
                continue;

            var utilizador = new Utilizador
            {
                Name = i <= 8 ? $"Utilizador Teste {i}" : faker.Name.FullName(),
                Email = email,
                Role = PapelUtilizador.Utilizador,
                IsActive = true,
                Nationality = Nacionalidades[SeedRandom.Next(Nacionalidades.Length)],
                CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 180)),
            };

            utilizador.PasswordHash = passwordHashingStrategy.HashPassword(utilizador, "User123!");

            utilizadores.Add(utilizador);
        }

        if (utilizadores.Count > 0)
        {
            await db.Utilizadores.AddRangeAsync(utilizadores);
            await db.SaveChangesAsync();
        }

        return await db.Utilizadores.Where(u => u.Email.EndsWith("@teste.pt")).ToListAsync();
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

    private static async Task<List<Filme>> CriarFilmesAsync(AppDbContext db, List<Genero> generos)
    {
        var filmesExistentes = await db.Filmes.ToListAsync();

        if (filmesExistentes.Count >= NumeroFilmes)
            return filmesExistentes;

        var faker = new Faker("pt_PT");
        var filmes = new List<Filme>();

        for (var i = filmesExistentes.Count + 1; i <= NumeroFilmes; i++)
        {
            var genero = EscolherAleatorio(generos, 1).Single();

            var tituloBase = TitulosFilmesSeed[(i - 1) % TitulosFilmesSeed.Length];
            var titulo = i <= TitulosFilmesSeed.Length ? tituloBase : $"{tituloBase} {i}";

            var classificacao = (SeedRandom.Next(55, 99) / 10.0m).ToString(
                "0.0",
                CultureInfo.InvariantCulture
            );

            filmes.Add(
                new Filme
                {
                    TmdbId = 100000 + i,
                    Titulo = titulo,
                    Sinopse = faker.Lorem.Paragraphs(2),
                    DataLancamento = DateTime.UtcNow.AddDays(-SeedRandom.Next(365, 365 * 25)),
                    Genero = genero.Name,
                    Classificacao = classificacao,
                    CapaUrl = $"https://picsum.photos/seed/filme-{i}/400/600",
                    VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                }
            );
        }

        if (filmes.Count > 0)
        {
            await db.Filmes.AddRangeAsync(filmes);
            await db.SaveChangesAsync();
        }

        return await db.Filmes.ToListAsync();
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

            if (associacoesFestival.Count > 0 && !associacoesFestival.Any(ff => ff.ElegivelPremiosPublico))
            {
                foreach (var associacao in associacoesFestival.Take(8))
                    associacao.ElegivelPremiosPublico = true;
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task<List<Sessao>> CriarSessoesAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes
    )
    {
        var sessoesExistentesPorFestival = await db
            .Sessoes.GroupBy(s => s.FestivalId)
            .Select(g => new { FestivalId = g.Key, Quantidade = g.Count() })
            .ToDictionaryAsync(x => x.FestivalId, x => x.Quantidade);

        var sessoesNovas = new List<Sessao>();

        foreach (var festival in festivais)
        {
            sessoesExistentesPorFestival.TryGetValue(festival.Id, out var quantidadeExistente);

            if (quantidadeExistente >= 6)
                continue;

            var filmeIdsDoFestival = await db
                .FestivalFilmes.Where(ff => ff.FestivalId == festival.Id)
                .Select(ff => ff.FilmeId)
                .ToListAsync();

            var filmesDoFestival = filmes.Where(f => filmeIdsDoFestival.Contains(f.Id)).ToList();

            if (filmesDoFestival.Count == 0)
                continue;

            var planos = CriarPlanoSessoes(festival);
            var quantidadeEmFalta = 6 - quantidadeExistente;

            foreach (var plano in planos.Take(quantidadeEmFalta))
            {
                var sessao = new Sessao
                {
                    FestivalId = festival.Id,
                    Tipo = TipoSessao.HorarioFixo,
                    Inicio = plano.Inicio,
                    Fim = plano.Fim,
                    TemChatAoVivo = plano.TemChatAoVivo,
                    Observacoes = plano.Observacoes,
                };

                var quantidadeFilmesSessao = SeedRandom.Next(
                    1,
                    Math.Min(5, filmesDoFestival.Count) + 1
                );
                var filmesDaSessao = EscolherAleatorio(filmesDoFestival, quantidadeFilmesSessao);
                var ordem = 1;

                foreach (var filme in filmesDaSessao)
                {
                    sessao.FilmesDaSessao.Add(
                        new SessaoFilme { FilmeId = filme.Id, Ordem = ordem++ }
                    );
                }

                sessoesNovas.Add(sessao);
            }
        }

        if (sessoesNovas.Count > 0)
        {
            await db.Sessoes.AddRangeAsync(sessoesNovas);
            await db.SaveChangesAsync();
        }

        return await db.Sessoes.Include(s => s.FilmesDaSessao).ToListAsync();
    }

    private static async Task<Sessao> CriarSessaoChatAoVivoTesteAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes
    )
    {
        var agora = DateTime.UtcNow;
        var inicio = agora.AddMinutes(-30);
        var fim = agora.AddHours(2);

        var festival =
            festivais.FirstOrDefault(f => f.StartDate <= agora && f.EndDate >= agora)
            ?? festivais.OrderBy(f => f.StartDate).First();

        if (festival.StartDate > inicio.Date)
            festival.StartDate = inicio.Date;

        if (festival.EndDate < fim.Date)
            festival.EndDate = fim.Date;

        var filmeId = await db
            .FestivalFilmes.Where(ff => ff.FestivalId == festival.Id)
            .Select(ff => (int?)ff.FilmeId)
            .FirstOrDefaultAsync();

        if (!filmeId.HasValue)
        {
            var filme = filmes.OrderBy(f => f.Id).First();

            await db.FestivalFilmes.AddAsync(
                new FestivalFilme
                {
                    FestivalId = festival.Id,
                    FilmeId = filme.Id,
                    ElegivelPremiosPublico = true,
                }
            );

            await db.SaveChangesAsync();
            filmeId = filme.Id;
        }

        var sessao = await db
            .Sessoes.Include(s => s.FilmesDaSessao)
            .FirstOrDefaultAsync(s => s.Observacoes != null && s.Observacoes.Contains(MarcadorSessaoChatTeste));

        if (sessao == null)
        {
            sessao = new Sessao { FestivalId = festival.Id };
            await db.Sessoes.AddAsync(sessao);
        }

        sessao.FestivalId = festival.Id;
        sessao.Tipo = TipoSessao.HorarioFixo;
        sessao.Inicio = inicio;
        sessao.Fim = fim;
        sessao.TemChatAoVivo = true;
        sessao.Observacoes =
            $"{MarcadorSessaoChatTeste} - Sessao sempre ativa para testar o chat ao vivo.";

        if (sessao.FilmesDaSessao.Count > 0)
            db.SessaoFilmes.RemoveRange(sessao.FilmesDaSessao);

        sessao.FilmesDaSessao = new List<SessaoFilme>
        {
            new()
            {
                Sessao = sessao,
                FilmeId = filmeId.Value,
                Ordem = 1,
                InicioOffsetSegundos = 0,
                IntervaloAposSegundos = 0,
            },
        };

        await db.SaveChangesAsync();

        return sessao;
    }

    private static List<(
        DateTime Inicio,
        DateTime Fim,
        string Observacoes,
        bool TemChatAoVivo
    )> CriarPlanoSessoes(Festival festival)
    {
        var agora = DateTime.UtcNow;
        var hoje = agora.Date;

        var festivalADecorrer = festival.StartDate.Date <= hoje && festival.EndDate.Date >= hoje;
        var festivalPassado = festival.EndDate.Date < hoje;

        if (festivalADecorrer)
        {
            return new List<(DateTime Inicio, DateTime Fim, string Observacoes, bool TemChatAoVivo)>
            {
                (
                    hoje.AddDays(-1).AddHours(20),
                    hoje.AddDays(-1).AddHours(23),
                    "Sessão recente realizada ontem, útil para testar histórico.",
                    true
                ),
                (
                    agora.AddHours(-4),
                    agora.AddHours(-2),
                    "Sessão recente terminada há pouco tempo.",
                    true
                ),
                (
                    agora.AddHours(-1),
                    agora.AddHours(2),
                    "Sessão a decorrer neste momento com chat ao vivo.",
                    true
                ),
                (
                    hoje.AddHours(21),
                    hoje.AddHours(23).AddMinutes(30),
                    "Sessão marcada para hoje à noite.",
                    true
                ),
                (
                    hoje.AddDays(1).AddHours(18),
                    hoje.AddDays(1).AddHours(20),
                    "Sessão futura próxima.",
                    true
                ),
                (
                    hoje.AddDays(2).AddHours(0),
                    hoje.AddDays(2).AddHours(23).AddMinutes(59),
                    "Janela de acesso diária simulada.",
                    false
                ),
            };
        }

        if (festivalPassado)
        {
            var basePassada = festival.StartDate.Date;

            return new List<(DateTime Inicio, DateTime Fim, string Observacoes, bool TemChatAoVivo)>
            {
                (
                    basePassada.AddHours(18),
                    basePassada.AddHours(20),
                    "Sessão passada de abertura.",
                    true
                ),
                (
                    basePassada.AddDays(1).AddHours(20),
                    basePassada.AddDays(1).AddHours(22),
                    "Sessão passada em horário fixo.",
                    true
                ),
                (
                    basePassada.AddDays(2).AddHours(21),
                    basePassada.AddDays(2).AddHours(23),
                    "Sessão passada de competição.",
                    true
                ),
                (
                    basePassada.AddDays(3).AddHours(0),
                    basePassada.AddDays(3).AddHours(23).AddMinutes(59),
                    "Janela de acesso já terminada.",
                    false
                ),
                (
                    basePassada.AddDays(4).AddHours(19),
                    basePassada.AddDays(4).AddHours(21),
                    "Sessão passada de encerramento.",
                    true
                ),
                (
                    basePassada.AddDays(5).AddHours(20),
                    basePassada.AddDays(5).AddHours(22),
                    "Repetição passada.",
                    false
                ),
            };
        }

        var baseFutura = festival.StartDate.Date;

        return new List<(DateTime Inicio, DateTime Fim, string Observacoes, bool TemChatAoVivo)>
        {
            (baseFutura.AddHours(18), baseFutura.AddHours(20), "Sessão futura de abertura.", true),
            (baseFutura.AddHours(21), baseFutura.AddHours(23), "Sessão futura de estreia.", true),
            (
                baseFutura.AddDays(1).AddHours(20),
                baseFutura.AddDays(1).AddHours(22),
                "Sessão futura em horário fixo.",
                true
            ),
            (
                baseFutura.AddDays(2).AddHours(0),
                baseFutura.AddDays(2).AddHours(23).AddMinutes(59),
                "Janela de acesso futura.",
                false
            ),
            (
                baseFutura.AddDays(3).AddHours(19),
                baseFutura.AddDays(3).AddHours(21),
                "Sessão futura de debate.",
                true
            ),
            (
                baseFutura.AddDays(4).AddHours(20),
                baseFutura.AddDays(4).AddHours(22),
                "Sessão futura de encerramento.",
                true
            ),
        };
    }

    private static async Task<List<Acesso>> CriarAcessosAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes,
        List<Sessao> sessoes
    )
    {
        var acessosExistentes = await db.Acessos.ToListAsync();
        var nomesExistentes = acessosExistentes.Select(a => a.Nome).ToHashSet();

        var acessos = new List<Acesso>();

        foreach (var sessao in sessoes)
        {
            var nome = $"Bilhete - Sessão {sessao.Id}";

            if (nomesExistentes.Contains(nome))
                continue;

            acessos.Add(
                new Acesso
                {
                    Nome = nome,
                    Descricao =
                        sessao.Inicio <= DateTime.UtcNow && sessao.Fim >= DateTime.UtcNow
                            ? "Bilhete válido para uma sessão que está a decorrer agora."
                            : "Bilhete válido para uma sessão específica.",
                    Tipo = TipoAcesso.BilheteSessao,
                    Preco = sessao.TemChatAoVivo ? 5.99m : 4.99m,
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
            var nomePasseCompleto = $"Passe Completo - {festival.Name}";

            if (!nomesExistentes.Contains(nomePasseCompleto))
            {
                acessos.Add(
                    new Acesso
                    {
                        Nome = nomePasseCompleto,
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

            var totalDias = Math.Max(1, (festival.EndDate.Date - festival.StartDate.Date).Days + 1);

            for (var i = 0; i < totalDias; i++)
            {
                var dia = festival.StartDate.Date.AddDays(i);
                var nomePasseDiario = $"Passe Diário - {festival.Name} - {dia:dd/MM/yyyy}";

                if (nomesExistentes.Contains(nomePasseDiario))
                    continue;

                acessos.Add(
                    new Acesso
                    {
                        Nome = nomePasseDiario,
                        Descricao = "Passe válido para todas as sessões de um dia do festival.",
                        Tipo = TipoAcesso.PasseDiario,
                        Preco = 9.99m,
                        SessaoId = null,
                        FestivalId = festival.Id,
                        FilmeId = null,
                        DataAcesso = dia,
                        DuracaoHoras = null,
                        IsAtivo = true,
                        CriadoEm = DateTime.UtcNow,
                    }
                );
            }
        }

        var quantidadeAlugueres = Math.Max(40, (int)(filmes.Count * 0.65));
        var filmesComAluguer = EscolherAleatorio(filmes, quantidadeAlugueres);

        foreach (var filme in filmesComAluguer)
        {
            var nome = $"Aluguer Digital - {filme.Titulo}";

            if (nomesExistentes.Contains(nome))
                continue;

            acessos.Add(
                new Acesso
                {
                    Nome = nome,
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

        if (acessos.Count > 0)
        {
            await db.Acessos.AddRangeAsync(acessos);
            await db.SaveChangesAsync();
        }

        return await db
            .Acessos.Include(a => a.Sessao)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .ToListAsync();
    }

    private static async Task CriarPerfisAsync(AppDbContext db, List<Utilizador> utilizadores)
    {
        var utilizadoresComPerfil = await db
            .PerfisUtilizador.Select(p => p.UtilizadorId)
            .ToListAsync();

        var perfis = new List<PerfilUtilizador>();

        foreach (var utilizador in utilizadores)
        {
            if (utilizadoresComPerfil.Contains(utilizador.Id))
                continue;

            perfis.Add(
                new PerfilUtilizador
                {
                    UtilizadorId = utilizador.Id,
                    Bio =
                        $"Perfil de {utilizador.Name}. Apaixonado por cinema, festivais online e novas descobertas cinematográficas.",
                    ProfileImageUrl = $"https://picsum.photos/seed/perfil-{utilizador.Id}/200/200",
                    Location = Localizacoes[SeedRandom.Next(Localizacoes.Length)],
                    IsPublic = SeedRandom.Next(1, 100) <= 85,
                    CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 120)),
                }
            );
        }

        if (perfis.Count > 0)
        {
            await db.PerfisUtilizador.AddRangeAsync(perfis);
            await db.SaveChangesAsync();
        }
    }

    private static async Task CriarGenerosFavoritosAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Genero> generos
    )
    {
        var favoritosExistentes = await db.UtilizadoresGenerosFavoritos.ToListAsync();

        var chavesExistentes = favoritosExistentes
            .Select(f => Chave(f.UtilizadorId, f.GeneroId))
            .ToHashSet();

        var favoritos = new List<UtilizadorGeneroFavorito>();

        foreach (var utilizador in utilizadores)
        {
            var favoritosAtuais = favoritosExistentes.Count(f => f.UtilizadorId == utilizador.Id);
            var quantidadePretendida = SeedRandom.Next(2, 6);
            var quantidadeEmFalta = Math.Max(0, quantidadePretendida - favoritosAtuais);

            if (quantidadeEmFalta == 0)
                continue;

            var generosEscolhidos = EscolherAleatorio(generos, quantidadeEmFalta + 3);

            foreach (var genero in generosEscolhidos)
            {
                var chave = Chave(utilizador.Id, genero.Id);

                if (chavesExistentes.Contains(chave))
                    continue;

                favoritos.Add(
                    new UtilizadorGeneroFavorito
                    {
                        UtilizadorId = utilizador.Id,
                        GeneroId = genero.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 90)),
                    }
                );

                chavesExistentes.Add(chave);

                if (favoritos.Count(f => f.UtilizadorId == utilizador.Id) >= quantidadeEmFalta)
                    break;
            }
        }

        if (favoritos.Count > 0)
        {
            await db.UtilizadoresGenerosFavoritos.AddRangeAsync(favoritos);
            await db.SaveChangesAsync();
        }
    }

    private static async Task<List<Comunidade>> CriarComunidadesAsync(
        AppDbContext db,
        List<Utilizador> utilizadores
    )
    {
        var existentes = await db.Comunidades.ToListAsync();

        if (existentes.Count >= NumeroComunidades)
            return existentes;

        var comunidades = new List<Comunidade>();

        for (var i = existentes.Count; i < NumeroComunidades; i++)
        {
            var nome = NomesComunidades[i % NomesComunidades.Length];

            if (existentes.Any(c => c.Name == nome) || comunidades.Any(c => c.Name == nome))
                nome = $"{nome} {i + 1}";

            comunidades.Add(
                new Comunidade
                {
                    Name = nome,
                    Description =
                        $"Espaço para discutir filmes, sessões, críticas e recomendações relacionadas com {nome}.",
                    ImageUrl = $"https://picsum.photos/seed/comunidade-{i + 1}/600/300",
                    IsPublic = SeedRandom.Next(1, 100) <= 90,
                    CodigoConvite = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
                    CreatedByUserId = EscolherAleatorio(utilizadores, 1).Single().Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 80)),
                }
            );
        }

        if (comunidades.Count > 0)
        {
            await db.Comunidades.AddRangeAsync(comunidades);
            await db.SaveChangesAsync();
        }

        return await db.Comunidades.ToListAsync();
    }

    private static async Task CriarMembrosComunidadesAsync(
        AppDbContext db,
        List<Comunidade> comunidades,
        List<Utilizador> utilizadores
    )
    {
        var existentes = await db.ComunidadeMembros.ToListAsync();

        var chavesExistentes = existentes
            .Select(m => Chave(m.ComunidadeId, m.UtilizadorId))
            .ToHashSet();

        var membros = new List<ComunidadeMembro>();

        foreach (var comunidade in comunidades)
        {
            var membrosAtuais = existentes.Count(m => m.ComunidadeId == comunidade.Id);
            var quantidadePretendida = SeedRandom.Next(8, Math.Min(22, utilizadores.Count) + 1);
            var quantidadeEmFalta = Math.Max(0, quantidadePretendida - membrosAtuais);

            if (quantidadeEmFalta == 0)
                continue;

            var utilizadoresEscolhidos = EscolherAleatorio(utilizadores, quantidadeEmFalta + 5);

            foreach (var utilizador in utilizadoresEscolhidos)
            {
                var chave = Chave(comunidade.Id, utilizador.Id);

                if (chavesExistentes.Contains(chave))
                    continue;

                membros.Add(
                    new ComunidadeMembro
                    {
                        ComunidadeId = comunidade.Id,
                        UtilizadorId = utilizador.Id,
                        Role = default,
                        JoinedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 90)),
                    }
                );

                chavesExistentes.Add(chave);

                if (membros.Count(m => m.ComunidadeId == comunidade.Id) >= quantidadeEmFalta)
                    break;
            }
        }

        if (membros.Count > 0)
        {
            await db.ComunidadeMembros.AddRangeAsync(membros);
            await db.SaveChangesAsync();
        }
    }

    private static async Task CriarComentariosAsync(
        AppDbContext db,
        List<Comunidade> comunidades,
        List<Utilizador> utilizadores
    )
    {
        var quantidadeExistente = await db.Comentarios.CountAsync();

        if (quantidadeExistente >= NumeroComentarios)
            return;

        var comentarios = new List<Comentario>();
        var quantidadeEmFalta = NumeroComentarios - quantidadeExistente;

        for (var i = 0; i < quantidadeEmFalta; i++)
        {
            var reportado = SeedRandom.Next(1, 100) <= 12;
            var visivel = !reportado || SeedRandom.Next(1, 100) <= 55;

            comentarios.Add(
                new Comentario
                {
                    UsuarioId = EscolherAleatorio(utilizadores, 1).Single().Id,
                    ComunidadeId = EscolherAleatorio(comunidades, 1).Single().Id,
                    Texto = TextosComentarios[SeedRandom.Next(TextosComentarios.Length)],
                    CriadoEm = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 60)),
                    Visivel = visivel,
                    Reportado = reportado,
                }
            );
        }

        await db.Comentarios.AddRangeAsync(comentarios);
        await db.SaveChangesAsync();
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

    private static async Task CriarCarrinhosAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Acesso> acessos
    )
    {
        var quantidadeExistente = await db.Carrinhos.CountAsync();

        if (quantidadeExistente >= 12)
            return;

        var utilizadoresComCarrinho = await db.Carrinhos.Select(c => c.UtilizadorId).ToListAsync();

        var candidatos = utilizadores.Where(u => !utilizadoresComCarrinho.Contains(u.Id)).ToList();

        var quantidadeEmFalta = 12 - quantidadeExistente;
        var utilizadoresEscolhidos = EscolherAleatorio(candidatos, quantidadeEmFalta);

        var carrinhos = new List<Carrinho>();

        foreach (var utilizador in utilizadoresEscolhidos)
        {
            var carrinho = new Carrinho
            {
                UtilizadorId = utilizador.Id,
                DataCriacao = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 12)),
                AtualizadoEm = DateTime.UtcNow.AddHours(-SeedRandom.Next(1, 72)),
            };

            var tiposPretendidos = new[]
            {
                TipoAcesso.BilheteSessao,
                TipoAcesso.PasseDiario,
                TipoAcesso.PasseCompleto,
                TipoAcesso.AluguerDigital,
            };

            var tiposEscolhidos = tiposPretendidos
                .OrderBy(_ => SeedRandom.Next())
                .Take(SeedRandom.Next(2, 5))
                .ToList();

            foreach (var tipo in tiposEscolhidos)
            {
                var acessosDoTipo = acessos.Where(a => a.Tipo == tipo && a.IsAtivo).ToList();

                if (acessosDoTipo.Count == 0)
                    continue;

                var acesso = EscolherAleatorio(acessosDoTipo, 1).Single();

                carrinho.Itens.Add(
                    new CarrinhoItem
                    {
                        AcessoId = acesso.Id,
                        PrecoUnitario = acesso.Preco,
                        Quantidade = 1,
                        DataAdicao = DateTime.UtcNow.AddHours(-SeedRandom.Next(1, 72)),
                    }
                );
            }

            if (carrinho.Itens.Any())
                carrinhos.Add(carrinho);
        }

        if (carrinhos.Count > 0)
        {
            await db.Carrinhos.AddRangeAsync(carrinhos);
            await db.SaveChangesAsync();
        }
    }

    private static async Task CriarComprasEAcessosUtilizadorAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Acesso> acessos
    )
    {
        var comprasExistentes = await db.Compras.CountAsync();

        if (comprasExistentes >= NumeroCompras)
            return;

        var acessosComNavegacoes = await db
            .Acessos.Include(a => a.Sessao)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .Where(a => a.IsAtivo)
            .ToListAsync();

        if (acessosComNavegacoes.Count == 0)
            return;

        var compras = new List<Compra>();
        var acessosUtilizador = new List<AcessoUtilizador>();

        var quantidadeEmFalta = NumeroCompras - comprasExistentes;

        for (var i = 0; i < quantidadeEmFalta; i++)
        {
            var utilizador = EscolherAleatorio(utilizadores, 1).Single();
            var acessosComprados = EscolherAleatorio(acessosComNavegacoes, SeedRandom.Next(1, 5));

            var dataCompra = DateTime
                .UtcNow.AddDays(-SeedRandom.Next(0, 100))
                .AddMinutes(-SeedRandom.Next(1, 600));

            var compra = new Compra
            {
                UtilizadorId = utilizador.Id,
                Referencia =
                    $"CMP-SEED-{utilizador.Id}-{comprasExistentes + i + 1}-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                Estado = EstadoCompra.Pago,
                CriadaEm = dataCompra,
                PagaEm = dataCompra.AddMinutes(SeedRandom.Next(1, 20)),
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

                var validade = CalcularValidadeAcessoSeed(acesso, compra.PagaEm ?? compra.CriadaEm);

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
                        Ativo = validade.fim > DateTime.UtcNow,
                        CriadoEm = compra.CriadaEm,
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
                CriadoEm = compra.CriadaEm,
                ProcessadoEm = compra.PagaEm,
                Mensagem = "Pagamento seed aprovado automaticamente.",
            };

            compras.Add(compra);
        }

        if (compras.Count > 0)
        {
            await db.Compras.AddRangeAsync(compras);
            await db.AcessosUtilizador.AddRangeAsync(acessosUtilizador);
            await db.SaveChangesAsync();
        }
    }

    private static async Task GarantirAcessoSessaoChatTesteAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        Sessao sessaoChatTeste
    )
    {
        var utilizadorTeste =
            utilizadores.FirstOrDefault(u => u.Email == "utilizador1@teste.pt")
            ?? utilizadores.OrderBy(u => u.Id).FirstOrDefault();

        if (utilizadorTeste == null)
            return;

        var sessao = await db
            .Sessoes.AsNoTracking()
            .FirstAsync(s => s.Id == sessaoChatTeste.Id);

        var acesso = await db.Acessos.FirstOrDefaultAsync(a => a.Nome == "Bilhete - Chat Ao Vivo Teste");

        if (acesso == null)
        {
            acesso = new Acesso { Nome = "Bilhete - Chat Ao Vivo Teste", CriadoEm = DateTime.UtcNow };
            await db.Acessos.AddAsync(acesso);
        }

        acesso.Descricao = "Bilhete seed para testar o chat ao vivo da sessao ativa.";
        acesso.Tipo = TipoAcesso.BilheteSessao;
        acesso.Preco = 0m;
        acesso.PrecoPago = 0m;
        acesso.IsAtivo = true;
        acesso.SessaoId = sessao.Id;
        acesso.FestivalId = null;
        acesso.FilmeId = null;
        acesso.DataAcesso = null;
        acesso.DuracaoHoras = null;
        acesso.Validade = sessao.Fim;

        await db.SaveChangesAsync();

        var referenciaCompra = $"CMP-CHAT-ATIVO-TESTE-{utilizadorTeste.Id}";
        var referenciaPagamento = $"PG-{referenciaCompra}";
        var compra = await db
            .Compras.Include(c => c.Itens)
            .Include(c => c.Pagamento)
            .FirstOrDefaultAsync(c => c.Referencia == referenciaCompra);

        if (compra == null)
        {
            compra = new Compra
            {
                Referencia = referenciaCompra,
                UtilizadorId = utilizadorTeste.Id,
            };

            await db.Compras.AddAsync(compra);
        }

        compra.Estado = EstadoCompra.Pago;
        compra.CriadaEm = DateTime.UtcNow.AddMinutes(-35);
        compra.PagaEm = DateTime.UtcNow.AddMinutes(-34);
        compra.ValorTotal = 0m;

        var item = compra.Itens.FirstOrDefault(i => i.AcessoId == acesso.Id);

        if (item == null)
        {
            compra.Itens.Add(
                new ItemCompra
                {
                    AcessoId = acesso.Id,
                    NomeAcesso = acesso.Nome,
                    TipoAcesso = acesso.Tipo,
                    PrecoUnitario = 0m,
                    Quantidade = 1,
                    Subtotal = 0m,
                }
            );
        }
        else
        {
            item.NomeAcesso = acesso.Nome;
            item.TipoAcesso = acesso.Tipo;
            item.PrecoUnitario = 0m;
            item.Quantidade = 1;
            item.Subtotal = 0m;
        }

        compra.Pagamento ??= new Pagamento { Referencia = referenciaPagamento };
        compra.Pagamento.Referencia = referenciaPagamento;
        compra.Pagamento.Valor = 0m;
        compra.Pagamento.Metodo = "Seed";
        compra.Pagamento.Estado = EstadoPagamento.Aprovado;
        compra.Pagamento.CriadoEm = compra.CriadaEm;
        compra.Pagamento.ProcessadoEm = compra.PagaEm;
        compra.Pagamento.Mensagem = "Acesso seed para teste do chat ao vivo.";

        await db.SaveChangesAsync();

        var acessoUtilizador = await db.AcessosUtilizador.FirstOrDefaultAsync(a =>
            a.UtilizadorId == utilizadorTeste.Id && a.AcessoId == acesso.Id
        );

        if (acessoUtilizador == null)
        {
            acessoUtilizador = new AcessoUtilizador
            {
                UtilizadorId = utilizadorTeste.Id,
                AcessoId = acesso.Id,
                CompraId = compra.Id,
                CriadoEm = DateTime.UtcNow,
            };

            await db.AcessosUtilizador.AddAsync(acessoUtilizador);
        }

        acessoUtilizador.CompraId = compra.Id;
        acessoUtilizador.TipoAcesso = TipoAcesso.BilheteSessao;
        acessoUtilizador.SessaoId = sessao.Id;
        acessoUtilizador.FestivalId = sessao.FestivalId;
        acessoUtilizador.FilmeId = null;
        acessoUtilizador.InicioValidade = sessao.Inicio.AddMinutes(-15);
        acessoUtilizador.FimValidade = sessao.Fim;
        acessoUtilizador.Ativo = true;

        await db.SaveChangesAsync();
    }

    private static (DateTime inicio, DateTime fim) CalcularValidadeAcessoSeed(
        Acesso acesso,
        DateTime dataCompra
    )
    {
        return acesso.Tipo switch
        {
            TipoAcesso.BilheteSessao when acesso.Sessao != null => (
                acesso.Sessao.Inicio,
                acesso.Sessao.Fim
            ),

            TipoAcesso.PasseDiario when acesso.DataAcesso.HasValue => (
                acesso.DataAcesso.Value.Date,
                acesso.DataAcesso.Value.Date.AddDays(1).AddTicks(-1)
            ),

            TipoAcesso.PasseCompleto when acesso.Festival != null => (
                acesso.Festival.StartDate.Date,
                acesso.Festival.EndDate.Date.AddDays(1).AddTicks(-1)
            ),

            TipoAcesso.AluguerDigital => (
                dataCompra,
                dataCompra.AddHours(acesso.DuracaoHoras ?? 48)
            ),

            _ => (dataCompra, dataCompra.AddDays(1)),
        };
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
