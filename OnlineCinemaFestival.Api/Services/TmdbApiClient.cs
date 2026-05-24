using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.Configuracao;

namespace OnlineCinemaFestival.Api.Services;

public class TmdbApiClient : ITmdbApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TmdbOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public TmdbApiClient(HttpClient httpClient, IOptions<TmdbOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        var token = _options.Token;
        var baseUrl = _options.BaseUrl;

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
