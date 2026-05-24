using Bogus;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public static partial class DbSeeder
{
    // ── Constantes globais ─────────────────────────────────────────────────────
    private const int NumeroUtilizadores = 35;
    private const int NumeroFilmes = 20;
    private const int NumeroComunidades = 10;
    private const int NumeroComentarios = 180;
    private const int NumeroCompras = 60;
    private const int NumeroVisualizacoes = 220;
    private const string MarcadorSessaoChatTeste = "CHAT_ATIVO_TESTE";

    private static readonly Random SeedRandom = new(2120622);

    // ── Dados estáticos de seed ───────────────────────────────────────────────
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

    private static readonly FilmeTmdbSeed[] FilmesTmdbSeed =
    {
        new(550,    "Fight Club",                                    "Fight Club",                                    "Drama",                           "1999-10-15", 139, 8.4, "/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg",  "David Fincher"),
        new(155,    "The Dark Knight",                               "The Dark Knight",                               "Acao, Crime, Drama",              "2008-07-18", 152, 8.5, "/qJ2tW6WMUDux911r6m7haRef0WH.jpg",  "Christopher Nolan"),
        new(680,    "Pulp Fiction",                                  "Pulp Fiction",                                  "Crime, Drama",                    "1994-10-14", 154, 8.5, "/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg",  "Quentin Tarantino"),
        new(13,     "Forrest Gump",                                  "Forrest Gump",                                  "Drama, Romance",                  "1994-07-06", 142, 8.5, "/arw2vcBveWOVZr6pxd9XTd1TdQa.jpg",  "Robert Zemeckis"),
        new(238,    "The Godfather",                                  "The Godfather",                                  "Crime, Drama",                    "1972-03-24", 175, 8.7, "/3bhkrj58Vtu7enYsRolD1fZdja1.jpg",  "Francis Ford Coppola"),
        new(278,    "The Shawshank Redemption",                      "The Shawshank Redemption",                      "Drama",                           "1994-09-23", 142, 8.7, "/9cqNxx0GxF0bflZmeSMuL5tnGzr.jpg",  "Frank Darabont"),
        new(27205,  "Inception",                                     "Inception",                                     "Acao, Ficcao Cientifica",         "2010-07-16", 148, 8.4, "/oYuLEt3zVCKq57qu2F8dT7NIa6f.jpg",  "Christopher Nolan"),
        new(603,    "The Matrix",                                    "The Matrix",                                    "Acao, Ficcao Cientifica",         "1999-03-31", 136, 8.2, "/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg",  "Lana Wachowski, Lilly Wachowski"),
        new(129,    "Spirited Away",                                  "Sen to Chihiro no Kamikakushi",                 "Animacao, Fantasia",              "2001-07-20", 125, 8.5, "/39wmItIWsg5sZMyRUHLkWBcuVCM.jpg",  "Hayao Miyazaki"),
        new(496243, "Parasite",                                      "Gisaengchung",                                  "Thriller, Drama",                 "2019-05-30", 133, 8.5, "/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg",  "Bong Joon-ho"),
        new(24428,  "The Avengers",                                  "The Avengers",                                  "Acao, Aventura",                  "2012-05-04", 143, 7.7, "/RYMX2wcKCBAr24UyPD7xwmjaTn.jpg",  "Joss Whedon"),
        new(299536, "Avengers: Infinity War",                        "Avengers: Infinity War",                        "Acao, Aventura",                  "2018-04-27", 149, 8.2, "/7WsyChQLEftFiDOVTGkv3hFpyyt.jpg",  "Anthony Russo, Joe Russo"),
        new(157336, "Interstellar",                                  "Interstellar",                                  "Aventura, Drama, Ficcao Cientifica","2014-11-07",169, 8.4, "/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg",  "Christopher Nolan"),
        new(19404,  "Dilwale Dulhania Le Jayenge",                   "Dilwale Dulhania Le Jayenge",                   "Comedia, Drama, Romance",         "1995-10-20", 190, 8.5, "/ktejodbcdCPXbMMdnpI9BUxW6O8.jpg",  "Aditya Chopra"),
        new(497,    "The Green Mile",                                "The Green Mile",                                "Fantasia, Drama",                 "1999-12-10", 189, 8.5, "/8VG8fDNiy50H4FedGwdSVUPoaJe.jpg",  "Frank Darabont"),
        new(372058, "Your Name.",                                    "Kimi no Na wa.",                                "Animacao, Romance, Drama",        "2016-08-26", 106, 8.5, "/vfJFJPepRKapMd5G2ro7klIRysq.jpg",  "Makoto Shinkai"),
        new(475557, "Joker",                                         "Joker",                                         "Crime, Thriller, Drama",          "2019-10-04", 122, 8.2, "/udDclJoHjfjb8Ekgsd4FDteOkCU.jpg",  "Todd Phillips"),
        new(324857, "Spider-Man: Into the Spider-Verse",             "Spider-Man: Into the Spider-Verse",             "Animacao, Acao",                  "2018-12-14", 117, 8.4, "/iiZZdoQBEYBv6id8su7ImL0oCbD.jpg",  "Bob Persichetti, Peter Ramsey, Rodney Rothman"),
        new(120,    "The Lord of the Rings: The Fellowship of the Ring", "The Lord of the Rings: The Fellowship of the Ring", "Aventura, Fantasia", "2001-12-19", 179, 8.4, "/6oom5QYQ2yQTMJIbnvbkBL9cHo6.jpg", "Peter Jackson"),
        new(1891,   "The Empire Strikes Back",                       "The Empire Strikes Back",                       "Aventura, Acao, Ficcao Cientifica","1980-05-21",124, 8.4, "/nNAeTmF4CtdSgMDplXTDPOpYzsX.jpg",  "Irvin Kershner"),
    };

    // ── Utilitários partilhados ───────────────────────────────────────────────
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

    private static string Chave(params object?[] valores) =>
        string.Join(":", valores.Select(v => v?.ToString() ?? ""));

    private static string ObterConfiguracaoObrigatoria(IConfiguration configuration, string chave) =>
        configuration[chave]
        ?? throw new InvalidOperationException($"{chave} nao configurado na configuracao.");

    // ── Tipos privados ────────────────────────────────────────────────────────
    private sealed record FilmeTmdbSeed(
        int TmdbId,
        string Titulo,
        string TituloOriginal,
        string Genero,
        string DataLancamento,
        int DuracaoMinutos,
        double AvaliacaoTmdb,
        string CapaPath,
        string Realizador
    );
}
