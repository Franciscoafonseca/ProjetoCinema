using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class AvaliacaoService
{
    private readonly HttpClient _http;

    public AvaliacaoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<AvaliacaoDTO> CriarAsync(int filmeId, CriarAvaliacaoDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync($"api/filmes/{filmeId}/reviews", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));

        return await resposta.Content.ReadFromJsonAsync<AvaliacaoDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<AvaliacaoDTO> AtualizarMinhaAsync(int filmeId, CriarAvaliacaoDTO dto)
    {
        var resposta = await _http.PutAsJsonAsync($"api/filmes/{filmeId}/reviews/minha", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));

        return await resposta.Content.ReadFromJsonAsync<AvaliacaoDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    private static async Task<string> ObterMensagemErroAsync(HttpResponseMessage resposta)
    {
        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            return "Tens de iniciar sessao para avaliar.";

        if (resposta.StatusCode == HttpStatusCode.Forbidden)
            return "So podes avaliar este filme depois de o veres atraves da plataforma.";

        var conteudo = await resposta.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(conteudo))
            return "Nao foi possivel publicar a review.";

        try
        {
            using var doc = JsonDocument.Parse(conteudo);
            if (doc.RootElement.TryGetProperty("errors", out var errors))
            {
                var mensagens = errors
                    .EnumerateObject()
                    .SelectMany(prop => prop.Value.EnumerateArray().Select(v => v.GetString()))
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList();

                if (mensagens.Count > 0)
                    return string.Join(" ", mensagens);
            }
        }
        catch
        {
        }

        return conteudo.Trim('"');
    }
}
