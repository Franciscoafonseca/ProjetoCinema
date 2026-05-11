using Microsoft.JSInterop;

namespace OnlineCinemaFestival.Client.Services;

public class ArmazenamentoToken
{
    private const string Chave = "cinema_token";
    private readonly IJSRuntime _js;

    public ArmazenamentoToken(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> ObterAsync()
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", Chave);
    }

    public async Task GuardarAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", Chave, token);
    }

    public async Task RemoverAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", Chave);
    }
}
