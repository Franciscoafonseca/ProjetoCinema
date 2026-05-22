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
                await MensagemErroApi.ObterAsync(
                    resposta,
                    "Nao foi possivel carregar comunidades publicas."
                )
            );

        var comunidades = await resposta.Content.ReadFromJsonAsync<List<ComunidadeDTO>>() ?? new();
        foreach (var comunidade in comunidades)
            NormalizarFotosMembros(comunidade);
        return comunidades;
    }

    public async Task<List<ComunidadeDTO>> ObterMinhasAsync()
    {
        var resposta = await _http.GetAsync("api/comunidades/minhas");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(
                    resposta,
                    "Nao foi possivel carregar as tuas comunidades."
                )
            );

        var comunidades = await resposta.Content.ReadFromJsonAsync<List<ComunidadeDTO>>() ?? new();
        foreach (var comunidade in comunidades)
            NormalizarFotosMembros(comunidade);
        return comunidades;
    }

    public async Task<ComunidadeDTO> ObterPorIdAsync(Guid id)
    {
        var resposta = await _http.GetAsync($"api/comunidades/{id}");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel abrir a comunidade.")
            );

        return NormalizarFotosMembros(await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>())
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<ComunidadeDTO> CriarAsync(ComunidadeCreateDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync("api/comunidades", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel criar a comunidade.")
            );

        return NormalizarFotosMembros(await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>())
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
        var resposta = await _http.GetAsync(
            $"api/comunidades/convite/{Uri.EscapeDataString(codigo.Trim())}"
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Convite nao encontrado.")
            );

        return NormalizarFotosMembros(await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>())
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

    private ComunidadeDTO? NormalizarFotosMembros(ComunidadeDTO? comunidade)
    {
        if (comunidade == null)
            return comunidade;

        foreach (var membro in comunidade.Members)
        {
            if (string.IsNullOrWhiteSpace(membro.ProfileImageUrl))
                continue;

            if (Uri.TryCreate(membro.ProfileImageUrl, UriKind.Absolute, out _))
                continue;

            var baseUri = _http.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
            membro.ProfileImageUrl = $"{baseUri}{membro.ProfileImageUrl}";
        }

        return comunidade;
    }
}
