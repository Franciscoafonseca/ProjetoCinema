using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ComunidadeService
{
    private readonly HttpClient _http;

    public ComunidadeService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ComunidadeDTO>> ObterPublicasAsync()
    {
        var resposta = await _http.GetAsync("api/comunidades");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel carregar comunidades publicas.")
            );

        return await resposta.Content.ReadFromJsonAsync<List<ComunidadeDTO>>() ?? new();
    }

    public async Task<List<ComunidadeDTO>> ObterMinhasAsync()
    {
        var resposta = await _http.GetAsync("api/comunidades/minhas");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel carregar as tuas comunidades.")
            );

        return await resposta.Content.ReadFromJsonAsync<List<ComunidadeDTO>>() ?? new();
    }

    public async Task<ComunidadeDTO> ObterPorIdAsync(Guid id)
    {
        var resposta = await _http.GetAsync($"api/comunidades/{id}");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel abrir a comunidade.")
            );

        return await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<ComunidadeDTO> CriarAsync(ComunidadeCreateDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync("api/comunidades", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel criar a comunidade.")
            );

        return await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task AderirAsync(Guid id)
    {
        var resposta = await _http.PostAsync($"api/comunidades/{id}/aderir", null);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel aderir a comunidade.")
            );
    }

    public async Task<ComunidadeDTO> ObterPorConviteAsync(string codigo)
    {
        var resposta = await _http.GetAsync($"api/comunidades/convite/{Uri.EscapeDataString(codigo.Trim())}");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Convite nao encontrado.")
            );

        return await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task AderirPorConviteAsync(string codigo)
    {
        var resposta = await _http.PostAsync(
            $"api/comunidades/convite/{Uri.EscapeDataString(codigo.Trim())}/aderir",
            null
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel aceitar o convite.")
            );
    }
}
