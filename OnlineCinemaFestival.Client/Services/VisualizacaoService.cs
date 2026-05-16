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

    public async Task<VisualizacaoDTO> ObterFilmeAsync(int filmeId, int? festivalId = null)
    {
        var url = festivalId.HasValue
            ? $"api/visualizacao/filme/{filmeId}?festivalId={festivalId.Value}"
            : $"api/visualizacao/filme/{filmeId}";

        return await ObterVisualizacaoAsync(url);
    }

    public async Task<VisualizacaoDTO> ObterSessaoAsync(int sessaoId)
    {
        return await ObterVisualizacaoAsync($"api/visualizacao/sessao/{sessaoId}");
    }

    public async Task<List<VisualizacaoHistoricoDTO>> ObterHistoricoAsync()
    {
        return await _http.GetFromJsonAsync<List<VisualizacaoHistoricoDTO>>(
                "api/visualizacao/historico"
            ) ?? new();
    }

    private async Task<VisualizacaoDTO> ObterVisualizacaoAsync(string url)
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

        return await resposta.Content.ReadFromJsonAsync<VisualizacaoDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }
}
