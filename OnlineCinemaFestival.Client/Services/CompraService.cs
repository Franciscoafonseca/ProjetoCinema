using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class CompraService
{
    private readonly HttpClient _http;

    public CompraService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CompraDto> FinalizarCompraAsync()
    {
        var resposta = await _http.PostAsync("api/checkout", null);

        if (!resposta.IsSuccessStatusCode)
        {
            var conteudo = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(conteudo)
                    ? "Nao foi possivel finalizar a compra."
                    : conteudo
            );
        }

        return await resposta.Content.ReadFromJsonAsync<CompraDto>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<List<CompraDto>> ObterMinhasAsync()
    {
        return await _http.GetFromJsonAsync<List<CompraDto>>("api/compras/minhas") ?? new();
    }
}
