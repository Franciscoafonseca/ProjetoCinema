using System.Net;
using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class CarrinhoService
{
    private readonly HttpClient _http;

    public CarrinhoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CarrinhoDto> ObterAsync()
    {
        return await _http.GetFromJsonAsync<CarrinhoDto>("api/carrinho") ?? new CarrinhoDto();
    }

    public async Task<CarrinhoResumoDto> ObterResumoAsync()
    {
        return await _http.GetFromJsonAsync<CarrinhoResumoDto>("api/carrinho/resumo")
            ?? new CarrinhoResumoDto();
    }

    public async Task<CarrinhoValidacaoDto> ValidarAsync()
    {
        var resposta = await _http.PostAsync("api/carrinho/validar", null);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));

        return await resposta.Content.ReadFromJsonAsync<CarrinhoValidacaoDto>()
            ?? new CarrinhoValidacaoDto();
    }

    public async Task<CarrinhoDto> AdicionarItemAsync(int acessoId, int quantidade = 1)
    {
        var resposta = await _http.PostAsJsonAsync(
            "api/carrinho/itens",
            new AdicionarItemCarrinhoDto { AcessoId = acessoId, Quantidade = quantidade }
        );

        return await LerCarrinhoAsync(resposta);
    }

    public Task<CarrinhoDto> AdicionarBilheteSessaoAsync(int sessaoId, int quantidade = 1)
    {
        return AdicionarPorTipoAsync(
            new CarrinhoItemCreateDto
            {
                TipoAcesso = TiposAcesso.BilheteSessao,
                SessaoId = sessaoId,
                Quantidade = quantidade,
            }
        );
    }

    public Task<CarrinhoDto> AdicionarPasseDiarioAsync(int festivalId, DateTime dataPasse)
    {
        return AdicionarPorTipoAsync(
            new CarrinhoItemCreateDto
            {
                TipoAcesso = TiposAcesso.PasseDiario,
                FestivalId = festivalId,
                DataPasse = dataPasse.Date,
                Quantidade = 1,
            }
        );
    }

    public Task<CarrinhoDto> AdicionarPasseCompletoAsync(int festivalId)
    {
        return AdicionarPorTipoAsync(
            new CarrinhoItemCreateDto
            {
                TipoAcesso = TiposAcesso.PasseCompleto,
                FestivalId = festivalId,
                Quantidade = 1,
            }
        );
    }

    public Task<CarrinhoDto> AdicionarAluguerDigitalAsync(int filmeId)
    {
        return AdicionarPorTipoAsync(
            new CarrinhoItemCreateDto
            {
                TipoAcesso = TiposAcesso.AluguerDigital,
                FilmeId = filmeId,
                Quantidade = 1,
            }
        );
    }

    public async Task<CarrinhoDto> AtualizarQuantidadeAsync(int itemId, int quantidade)
    {
        var resposta = await _http.PutAsJsonAsync(
            $"api/carrinho/items/{itemId}",
            new CarrinhoItemUpdateDto { Quantidade = quantidade }
        );

        return await LerCarrinhoAsync(resposta);
    }

    public async Task RemoverItemAsync(int itemId)
    {
        var resposta = await _http.DeleteAsync($"api/carrinho/items/{itemId}");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));
    }

    public async Task LimparAsync()
    {
        var resposta = await _http.DeleteAsync("api/carrinho/limpar");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));
    }

    private async Task<CarrinhoDto> AdicionarPorTipoAsync(CarrinhoItemCreateDto dto)
    {
        var resposta = await _http.PostAsJsonAsync("api/carrinho/items", dto);
        return await LerCarrinhoAsync(resposta);
    }

    private static async Task<CarrinhoDto> LerCarrinhoAsync(HttpResponseMessage resposta)
    {
        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));

        return await resposta.Content.ReadFromJsonAsync<CarrinhoDto>() ?? new CarrinhoDto();
    }

    private static async Task<string> ObterMensagemErroAsync(HttpResponseMessage resposta)
    {
        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            return "Tens de iniciar sessao para usar o carrinho.";

        if (resposta.StatusCode == HttpStatusCode.Forbidden)
            return "Nao tens permissao para esta operacao.";

        var conteudo = await resposta.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(conteudo)
            ? "Nao foi possivel atualizar o carrinho."
            : conteudo.Trim('"');
    }
}
