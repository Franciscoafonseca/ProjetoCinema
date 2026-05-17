using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class PremioFestivalService
{
    private readonly HttpClient _http;

    public PremioFestivalService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PremioFestivalDTO>> ObterPremiosPorFestivalAsync(int festivalId)
    {
        return await _http.GetFromJsonAsync<List<PremioFestivalDTO>>(
                $"api/festivals/{festivalId}/premios"
            )
            ?? new();
    }

    public async Task<List<ResultadoPremioFestivalDTO>> ObterResultadosPorFestivalAsync(
        int festivalId
    )
    {
        return await _http.GetFromJsonAsync<List<ResultadoPremioFestivalDTO>>(
                $"api/festivals/{festivalId}/premios/resultados-publicos"
            )
            ?? new();
    }

    public async Task<List<ResultadoPremioFestivalDTO>> ObterResultadosPorFilmeAsync(int filmeId)
    {
        return await _http.GetFromJsonAsync<List<ResultadoPremioFestivalDTO>>(
                $"api/filmes/{filmeId}/premios-resultados"
            )
            ?? new();
    }

    public async Task<PremioFestivalDTO> CriarPremioAsync(
        int festivalId,
        CriarPremioFestivalDTO dto
    )
    {
        var resposta = await _http.PostAsJsonAsync($"api/festivals/{festivalId}/premios", dto);
        return await LerRespostaAsync<PremioFestivalDTO>(resposta);
    }

    public async Task<PremioFestivalDTO> AbrirVotacaoAsync(int premioFestivalId)
    {
        var resposta = await _http.PostAsync(
            $"api/premios-festival/{premioFestivalId}/abrir",
            null
        );
        return await LerRespostaAsync<PremioFestivalDTO>(resposta);
    }

    public async Task VotarAsync(int premioFestivalId, int filmeId)
    {
        var resposta = await _http.PostAsJsonAsync(
            $"api/premios-festival/{premioFestivalId}/votos",
            new VotarPremioFestivalDTO { FilmeId = filmeId }
        );

        await GarantirSucessoAsync(resposta);
    }

    public async Task<PremioFestivalDTO> FecharVotacaoAsync(int premioFestivalId)
    {
        var resposta = await _http.PostAsync(
            $"api/premios-festival/{premioFestivalId}/fechar",
            null
        );
        return await LerRespostaAsync<PremioFestivalDTO>(resposta);
    }

    public async Task<ResultadoPremioFestivalDTO> PublicarResultadosAsync(int premioFestivalId)
    {
        var resposta = await _http.PostAsync(
            $"api/premios-festival/{premioFestivalId}/publicar",
            null
        );
        return await LerRespostaAsync<ResultadoPremioFestivalDTO>(resposta);
    }

    private static async Task<T> LerRespostaAsync<T>(HttpResponseMessage resposta)
    {
        await GarantirSucessoAsync(resposta);

        return await resposta.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    private static async Task GarantirSucessoAsync(HttpResponseMessage resposta)
    {
        if (resposta.IsSuccessStatusCode)
            return;

        var conteudo = await resposta.Content.ReadAsStringAsync();
        throw new InvalidOperationException(
            string.IsNullOrWhiteSpace(conteudo)
                ? "Nao foi possivel concluir a operacao."
                : conteudo.Trim('"')
        );
    }
}
