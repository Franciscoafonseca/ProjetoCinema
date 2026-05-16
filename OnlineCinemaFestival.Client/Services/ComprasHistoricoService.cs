using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ComprasHistoricoService
{
    private readonly HttpClient _http;

    public ComprasHistoricoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CompraHistoricoDto>> GetHistoricoAsync(string utilizadorId)
    {
        var response = await _http.GetFromJsonAsync<List<CompraHistoricoDto>>(
            $"api/compras/historico/{utilizadorId}");
        return response ?? new List<CompraHistoricoDto>();
    }
}
