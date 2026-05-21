using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ComentarioService
{
    private readonly HttpClient _http;

    public ComentarioService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ComentarioDTO>> ObterDaComunidadeAsync(Guid comunidadeId)
    {
        var resposta = await _http.GetAsync($"api/comunidades/{comunidadeId}/comentarios");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel carregar comentarios.")
            );

        return await resposta.Content.ReadFromJsonAsync<List<ComentarioDTO>>() ?? new();
    }

    public async Task<ComentarioDTO> CriarNaComunidadeAsync(Guid comunidadeId, ComentarioCreateDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync(
            $"api/comunidades/{comunidadeId}/comentarios",
            dto
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel publicar o comentario.")
            );

        return await resposta.Content.ReadFromJsonAsync<ComentarioDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }
}
