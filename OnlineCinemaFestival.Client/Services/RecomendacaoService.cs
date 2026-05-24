using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class RecomendacaoService
{
    private readonly HttpClient _http;

    public RecomendacaoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FilmeRecomendadoDTO>> ObterAsync(int quantidade = 12)
    {
        return await _http.GetFromJsonAsync<List<FilmeRecomendadoDTO>>(
                $"api/recomendacoes?quantidade={quantidade}"
            )
            ?? new List<FilmeRecomendadoDTO>();
    }
}
