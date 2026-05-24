using System.Net;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace OnlineCinemaFestival.Client.Services;

public class ManipuladorTokenHttp : DelegatingHandler
{
    private readonly ArmazenamentoToken _armazenamento;
    private readonly EstadoAutenticacaoCustomizado _estadoAutenticacao;
    private readonly PerfilEstadoService _perfilEstado;
    private readonly NavigationManager _navigationManager;

    public ManipuladorTokenHttp(
        ArmazenamentoToken armazenamento,
        EstadoAutenticacaoCustomizado estadoAutenticacao,
        PerfilEstadoService perfilEstado,
        NavigationManager navigationManager
    )
    {
        _armazenamento = armazenamento;
        _estadoAutenticacao = estadoAutenticacao;
        _perfilEstado = perfilEstado;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var token = await _armazenamento.ObterAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resposta = await base.SendAsync(request, cancellationToken);

        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _armazenamento.RemoverAsync();
            _perfilEstado.Limpar();
            _estadoAutenticacao.NotificarTerminouSessao();

            if (!_navigationManager.Uri.Contains("/login", StringComparison.OrdinalIgnoreCase))
                _navigationManager.NavigateTo("/login", replace: true);
        }

        return resposta;
    }
}
