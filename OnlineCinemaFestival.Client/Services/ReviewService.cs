using System.Net;
using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ReviewService
{
    private readonly HttpClient _http;

    public ReviewService(HttpClient http)
    {
        _http = http;
    }

    public async Task<AvaliacaoDto> CriarAsync(int filmeId, CriarAvaliacaoDto dto)
    {
        var resposta = await _http.PostAsJsonAsync($"api/filmes/{filmeId}/reviews", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));

        return await resposta.Content.ReadFromJsonAsync<AvaliacaoDto>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    private static async Task<string> ObterMensagemErroAsync(HttpResponseMessage resposta)
    {
        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            return "Tens de iniciar sessao para avaliar.";

        if (resposta.StatusCode == HttpStatusCode.Forbidden)
            return "So podes avaliar este filme depois de o veres atraves da plataforma.";

        var conteudo = await resposta.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(conteudo)
            ? "Nao foi possivel publicar a review."
            : conteudo.Trim('"');
    }
}
