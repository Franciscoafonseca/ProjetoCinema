using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class VisualizacaoApiException : Exception
{
    public VisualizacaoApiException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}

public class VisualizacaoService
{
    private readonly HttpClient _http;

    public VisualizacaoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<VisualizacaoDto> ObterFilmeAsync(int filmeId, int? festivalId = null)
    {
        var url = festivalId.HasValue
            ? $"api/visualizacao/filme/{filmeId}?festivalId={festivalId.Value}"
            : $"api/visualizacao/filme/{filmeId}";

        return await ObterVisualizacaoAsync(url);
    }

    public async Task<VisualizacaoDto> ObterSessaoAsync(int sessaoId)
    {
        return await ObterVisualizacaoAsync($"api/visualizacao/sessao/{sessaoId}");
    }

    public async Task<List<VisualizacaoHistoricoDto>> ObterHistoricoAsync()
    {
        return await _http.GetFromJsonAsync<List<VisualizacaoHistoricoDto>>(
                "api/visualizacao/historico"
            ) ?? new();
    }

    private async Task<VisualizacaoDto> ObterVisualizacaoAsync(string url)
    {
        var resposta = await _http.GetAsync(url);

        if (!resposta.IsSuccessStatusCode)
        {
            var conteudo = await resposta.Content.ReadAsStringAsync();
            throw new VisualizacaoApiException(
                (int)resposta.StatusCode,
                string.IsNullOrWhiteSpace(conteudo) ? "Sem acesso valido ao conteudo." : conteudo
            );
        }

        return await resposta.Content.ReadFromJsonAsync<VisualizacaoDto>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }
}
