using System.Net;
using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ListaService
{
    private readonly HttpClient _http;

    public ListaService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ListaPessoalDto>> GetMinhasListasAsync()
    {
        var resposta = await _http.GetAsync("api/listas");

        if (!resposta.IsSuccessStatusCode)
            return new();

        return await resposta.Content.ReadFromJsonAsync<List<ListaPessoalDto>>() ?? new();
    }

    public async Task<ListaPessoalDto> CriarAsync(CriarListaRequest pedido)
    {
        var resposta = await _http.PostAsJsonAsync("api/listas", pedido);

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(mensagem)
                ? "Não foi possível criar a lista."
                : mensagem);
        }

        return await resposta.Content.ReadFromJsonAsync<ListaPessoalDto>()
            ?? throw new InvalidOperationException("Resposta inválida do servidor.");
    }

    public async Task<bool> AdicionarFilmeAsync(int listaId, int filmeId)
    {
        var resposta = await _http.PostAsync($"api/listas/{listaId}/filmes/{filmeId}", null);

        if (resposta.StatusCode == HttpStatusCode.Conflict)
            return false; // Já está na lista — não é erro

        resposta.EnsureSuccessStatusCode();
        return true;
    }

    public async Task RemoverFilmeAsync(int listaId, int filmeId)
    {
        var resposta = await _http.DeleteAsync($"api/listas/{listaId}/filmes/{filmeId}");

        if (resposta.StatusCode == HttpStatusCode.NotFound)
            return; // Já não está lá — idempotente

        resposta.EnsureSuccessStatusCode();
    }

    public async Task RemoverListaAsync(int listaId)
    {
        var resposta = await _http.DeleteAsync($"api/listas/{listaId}");

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(mensagem)
                ? "Não foi possível apagar a lista."
                : mensagem);
        }
    }
}
