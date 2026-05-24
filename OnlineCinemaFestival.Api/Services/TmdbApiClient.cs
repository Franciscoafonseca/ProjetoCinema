using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace OnlineCinemaFestival.Api.Services;

public class TmdbApiClient : ITmdbApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public TmdbApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        var token = _configuration["Tmdb:Token"];
        var baseUrl = _configuration["Tmdb:BaseUrl"];

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("Configuracao TMDB em falta.");

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}{path}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.SendAsync(request);
        }
        catch (TaskCanceledException)
        {
            return default;
        }
        catch (HttpRequestException)
        {
            return default;
        }

        if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.TooManyRequests)
            return default;

        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
    }
}
