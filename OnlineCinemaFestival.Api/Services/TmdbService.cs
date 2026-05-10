using System.Net.Http.Headers;
using System.Text.Json;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Serviço responsável pela comunicação com a API externa TMDb.
/// Permite pesquisar filmes e obter detalhes completos de um filme através do seu identificador TMDb.
/// </summary>
public class TmdbService : ITmdbService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Inicializa uma nova instância do serviço TMDb.
    /// </summary>
    /// <param name="httpClient">
    /// Cliente HTTP utilizado para enviar pedidos à API externa.
    /// </param>
    /// <param name="configuration">
    /// Configuração da aplicação, usada para obter o token e o URL base do TMDb.
    /// </param>
    public TmdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    /// <summary>
    /// Pesquisa filmes na API TMDb com base numa expressão de pesquisa.
    /// </summary>
    /// <param name="query">Texto usado para pesquisar filmes.</param>
    /// <returns>Lista de filmes encontrados no TMDb, convertidos para DTOs internos.</returns>
    public async Task<IEnumerable<TmdbFilmeDto>> SearchFilmesTmdbAsync(string query)
    {
        // Obtém o token de autenticação e o URL base da API TMDb.
        var token = _configuration["Tmdb:Token"];
        var baseUrl = _configuration["Tmdb:BaseUrl"];

        // Constrói o URL de pesquisa, escapando caracteres especiais do texto pesquisado.
        var url = $"{baseUrl}search/movie?query={Uri.EscapeDataString(query)}&language=pt-PT";

        // Cria o pedido HTTP e adiciona o token Bearer no cabeçalho de autorização.
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Envia o pedido para a API TMDb.
        var response = await _httpClient.SendAsync(request);

        // Lança uma exceção caso a resposta indique erro HTTP.
        response.EnsureSuccessStatusCode();

        // Lê a resposta JSON devolvida pela API.
        var jsonString = await response.Content.ReadAsStringAsync();

        // Permite desserializar propriedades JSON ignorando diferenças entre maiúsculas e minúsculas.
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Converte o JSON recebido para o modelo de resposta da pesquisa TMDb.
        var tmdbResult = JsonSerializer.Deserialize<TmdbSearchResponse>(jsonString, options);

        // Converte os resultados externos para DTOs usados internamente pela aplicação.
        return tmdbResult?.Results?.Select(FilmeMapper.MapFromTmdbResult)
            ?? Enumerable.Empty<TmdbFilmeDto>();
    }

    /// <summary>
    /// Obtém os detalhes de um filme através do seu identificador TMDb.
    /// </summary>
    /// <param name="tmdbId">Identificador do filme na API TMDb.</param>
    /// <returns>
    /// DTO com os dados detalhados do filme, ou null caso o filme não seja encontrado.
    /// </returns>
    public async Task<TmdbFilmeDto?> GetFilmeByTmdbIdAsync(int tmdbId)
    {
        // Obtém o token de autenticação e o URL base da API TMDb.
        var token = _configuration["Tmdb:Token"];
        var baseUrl = _configuration["Tmdb:BaseUrl"];

        // Constrói o URL para obter os detalhes de um filme específico.
        var url = $"{baseUrl}movie/{tmdbId}?language=pt-PT";

        // Cria o pedido HTTP com autenticação Bearer.
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Envia o pedido para a API externa.
        var response = await _httpClient.SendAsync(request);

        // Caso a API devolva erro, considera que o filme não foi encontrado.
        if (!response.IsSuccessStatusCode)
            return null;

        // Lê o conteúdo JSON devolvido pela API.
        var jsonString = await response.Content.ReadAsStringAsync();

        // Configura a desserialização para ignorar diferenças de capitalização nas propriedades.
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Converte o JSON recebido para o modelo de detalhes do filme.
        var filmeTmdb = JsonSerializer.Deserialize<TmdbMovieDetails>(jsonString, options);

        if (filmeTmdb == null)
            return null;

        // Converte os dados externos do TMDb para o DTO usado pela aplicação.
        return new TmdbFilmeDto
        {
            TmdbId = filmeTmdb.TmdbId,
            Titulo = filmeTmdb.Titulo,
            Sinopse = filmeTmdb.Sinopse,

            // Converte a data de lançamento para DateTime.
            // Caso a conversão falhe, usa DateTime.MinValue como valor padrão.
            DataLancamento = DateTime.TryParse(filmeTmdb.DataLancamento, out var date)
                ? date
                : DateTime.MinValue,

            // Constrói o URL completo da capa do filme, caso exista imagem disponível.
            CapaUrl = !string.IsNullOrWhiteSpace(filmeTmdb.CapaUrl)
                ? $"https://image.tmdb.org/t/p/w500{filmeTmdb.CapaUrl}"
                : "",

            // Formata a classificação com uma casa decimal.
            Classificacao = filmeTmdb.Classificacao?.ToString("0.0"),

            // Transforma a lista de géneros numa string separada por vírgulas.
            Genero =
                filmeTmdb.Genres != null && filmeTmdb.Genres.Any()
                    ? string.Join(", ", filmeTmdb.Genres.Select(g => g.Name))
                    : "Geral",
        };
    }
}
