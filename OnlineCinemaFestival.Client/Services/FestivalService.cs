using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class FestivalService
{
    private readonly HttpClient _http;

    public FestivalService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FestivalDTO>> ObterFestivaisAsync()
    {
        return await _http.GetFromJsonAsync<List<FestivalDTO>>("api/festivals") ?? new();
    }

    public async Task<FestivalDTO?> ObterFestivalPorIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<FestivalDTO>($"api/festivals/{id}");
    }

    public async Task<List<FilmeDTO>> ObterFilmesDoFestivalAsync(int festivalId)
    {
        return await _http.GetFromJsonAsync<List<FilmeDTO>>($"api/festivals/{festivalId}/filmes")
            ?? new();
    }

    public async Task<FestivalDTO> CriarAsync(CriarFestivalDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync("api/festivals", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await LerErroAsync(resposta, "Nao foi possivel criar o festival."));

        return await resposta.Content.ReadFromJsonAsync<FestivalDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<FestivalFilmeDTO> AssociarFilmeAsync(
        int festivalId,
        AssociarFilmeFestivalDTO dto
    )
    {
        var resposta = await _http.PostAsJsonAsync($"api/festivals/{festivalId}/filmes", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await LerErroAsync(resposta, "Nao foi possivel adicionar o filme ao festival."));

        return await resposta.Content.ReadFromJsonAsync<FestivalFilmeDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    private static async Task<string> LerErroAsync(HttpResponseMessage resposta, string fallback)
    {
        return await MensagemErroApi.ObterAsync(resposta, fallback);
    }
}
