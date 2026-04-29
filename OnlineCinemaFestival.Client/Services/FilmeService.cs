using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class FilmeService
{
    private readonly HttpClient _http;
    private const bool UseMock = true;

    public FilmeService(HttpClient http)
    {
        _http = http;
    }

    private static readonly List<FilmeDto> _mockFilmes = new()
    {
        new FilmeDto
        {
            Id = 1, Titulo = "Oppenheimer", Genero = "Drama",
            Realizador = "Christopher Nolan",
            Sinopse = "A história do físico J. Robert Oppenheimer e o seu papel no desenvolvimento da bomba atómica durante a Segunda Guerra Mundial.",
            UrlCartaz = "https://picsum.photos/seed/1/300/450",
            TrailerUrl = "", Popularidade = 9.4, ClassificacaoMedia = 8.9, Preco = 4.99m
        },
        new FilmeDto
        {
            Id = 2, Titulo = "Dune: Parte Dois", Genero = "Ficção Científica",
            Realizador = "Denis Villeneuve",
            Sinopse = "Paul Atreides une forças com os Fremen numa jornada de vingança contra os conspiradores que destruíram a sua família.",
            UrlCartaz = "https://picsum.photos/seed/2/300/450",
            TrailerUrl = "", Popularidade = 9.1, ClassificacaoMedia = 8.7, Preco = 4.99m
        },
        new FilmeDto
        {
            Id = 3, Titulo = "Barbie", Genero = "Comédia",
            Realizador = "Greta Gerwig",
            Sinopse = "Barbie e Ken embarcam numa aventura no mundo real depois de um súbito confronto com a existência e a mortalidade.",
            UrlCartaz = "https://picsum.photos/seed/3/300/450",
            TrailerUrl = "", Popularidade = 8.8, ClassificacaoMedia = 7.8, Preco = 3.99m
        },
        new FilmeDto
        {
            Id = 4, Titulo = "Poor Things", Genero = "Drama",
            Realizador = "Yorgos Lanthimos",
            Sinopse = "A incrível história de Bella Baxter, uma jovem trazida de volta à vida pelo brilhante e excêntrico cientista Dr. Godwin Baxter.",
            UrlCartaz = "https://picsum.photos/seed/4/300/450",
            TrailerUrl = "", Popularidade = 8.5, ClassificacaoMedia = 8.2, Preco = 3.99m
        },
        new FilmeDto
        {
            Id = 5, Titulo = "Saltburn", Genero = "Thriller",
            Realizador = "Emerald Fennell",
            Sinopse = "Um estudante de Oxford é convidado para passar o verão na extravagante propriedade da família do seu enigmático colega.",
            UrlCartaz = "https://picsum.photos/seed/5/300/450",
            TrailerUrl = "", Popularidade = 8.3, ClassificacaoMedia = 7.6, Preco = 3.99m
        },
        new FilmeDto
        {
            Id = 6, Titulo = "Anatomy of a Fall", Genero = "Thriller",
            Realizador = "Justine Triet",
            Sinopse = "Uma mulher é julgada pelo suspeito assassinato do marido. A única testemunha foi o seu filho de 11 anos.",
            UrlCartaz = "https://picsum.photos/seed/6/300/450",
            TrailerUrl = "", Popularidade = 8.6, ClassificacaoMedia = 8.3, Preco = 4.99m
        },
        new FilmeDto
        {
            Id = 7, Titulo = "Past Lives", Genero = "Romance",
            Realizador = "Celine Song",
            Sinopse = "Dois amigos de infância separados reencontram-se duas décadas depois em Nova Iorque, confrontando o destino e o amor.",
            UrlCartaz = "https://picsum.photos/seed/7/300/450",
            TrailerUrl = "", Popularidade = 8.4, ClassificacaoMedia = 8.0, Preco = 2.99m
        },
        new FilmeDto
        {
            Id = 8, Titulo = "Killers of the Flower Moon", Genero = "Drama",
            Realizador = "Martin Scorsese",
            Sinopse = "Os membros da nação Osage são assassinados em série após a descoberta de petróleo nas suas terras no início do século XX.",
            UrlCartaz = "https://picsum.photos/seed/8/300/450",
            TrailerUrl = "", Popularidade = 8.7, ClassificacaoMedia = 8.1, Preco = 4.99m
        },
        new FilmeDto
        {
            Id = 9, Titulo = "Society of the Snow", Genero = "Drama",
            Realizador = "J.A. Bayona",
            Sinopse = "Em 1972, um avião com uma equipa de râguebi uruguaia cai nos Andes. Os sobreviventes enfrentam condições extremas durante 72 dias.",
            UrlCartaz = "https://picsum.photos/seed/9/300/450",
            TrailerUrl = "", Popularidade = 9.0, ClassificacaoMedia = 8.6, Preco = 3.99m
        },
        new FilmeDto
        {
            Id = 10, Titulo = "RRR", Genero = "Ação",
            Realizador = "S.S. Rajamouli",
            Sinopse = "Uma história fictícia sobre dois lendários revolucionários indianos e a sua jornada antes de iniciarem a luta pela independência.",
            UrlCartaz = "https://picsum.photos/seed/10/300/450",
            TrailerUrl = "", Popularidade = 9.3, ClassificacaoMedia = 8.8, Preco = 2.99m
        },
        new FilmeDto
        {
            Id = 11, Titulo = "The Zone of Interest", Genero = "Drama",
            Realizador = "Jonathan Glazer",
            Sinopse = "O comandante de Auschwitz e a sua esposa constroem a sua vida de sonho ao lado do campo de concentração.",
            UrlCartaz = "https://picsum.photos/seed/11/300/450",
            TrailerUrl = "", Popularidade = 8.2, ClassificacaoMedia = 7.9, Preco = 3.99m
        },
        new FilmeDto
        {
            Id = 12, Titulo = "All of Us Strangers", Genero = "Romance",
            Realizador = "Andrew Haigh",
            Sinopse = "Um escritor de cinema que vive sozinho numa torre de apartamentos começa um romance improvável com um vizinho misterioso.",
            UrlCartaz = "https://picsum.photos/seed/12/300/450",
            TrailerUrl = "", Popularidade = 8.0, ClassificacaoMedia = 7.7, Preco = 2.99m
        },
    };

    public async Task<List<FilmeDto>> GetFilmesAsync(string? genero = null, string? search = null)
    {
        if (UseMock)
        {
            var resultado = _mockFilmes.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(genero))
                resultado = resultado.Where(f => f.Genero.Equals(genero, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(search))
                resultado = resultado.Where(f => f.Titulo.Contains(search, StringComparison.OrdinalIgnoreCase)
                                              || f.Realizador.Contains(search, StringComparison.OrdinalIgnoreCase));
            return await Task.FromResult(resultado.ToList());
        }

        var url = "api/filmes";
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(genero)) query.Add($"genero={Uri.EscapeDataString(genero)}");
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (query.Count > 0) url += "?" + string.Join("&", query);

        return await _http.GetFromJsonAsync<List<FilmeDto>>(url) ?? new();
    }

    public async Task<FilmeDto?> GetFilmeByIdAsync(int id)
    {
        if (UseMock)
            return await Task.FromResult(_mockFilmes.FirstOrDefault(f => f.Id == id));

        return await _http.GetFromJsonAsync<FilmeDto>($"api/filmes/{id}");
    }

    public async Task<List<string>> GetGenerosAsync()
    {
        if (UseMock)
            return await Task.FromResult(_mockFilmes.Select(f => f.Genero).Distinct().OrderBy(g => g).ToList());

        var filmes = await GetFilmesAsync();
        return filmes.Select(f => f.Genero).Distinct().OrderBy(g => g).ToList();
    }
}