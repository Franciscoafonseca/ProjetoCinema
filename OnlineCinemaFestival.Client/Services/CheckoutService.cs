using System.Net;
using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class CheckoutService
{
    private readonly HttpClient _http;

    public CheckoutService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CheckoutResultadoDto> FinalizarCompraAsync()
    {
        var resposta = await _http.PostAsync("api/checkout", null);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));

        return await resposta.Content.ReadFromJsonAsync<CheckoutResultadoDto>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    private static async Task<string> ObterMensagemErroAsync(HttpResponseMessage resposta)
    {
        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            return "Tens de iniciar sessao para finalizar a compra.";

        if (resposta.StatusCode == HttpStatusCode.Conflict)
            return await LerConteudoOuPadraoAsync(
                resposta,
                "Nao foi possivel finalizar a compra com os itens atuais."
            );

        return await LerConteudoOuPadraoAsync(resposta, "Nao foi possivel finalizar a compra.");
    }

    private static async Task<string> LerConteudoOuPadraoAsync(
        HttpResponseMessage resposta,
        string padrao
    )
    {
        var conteudo = await resposta.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(conteudo) ? padrao : conteudo.Trim('"');
    }
}
