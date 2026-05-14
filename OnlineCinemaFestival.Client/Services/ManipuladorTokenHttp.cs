using System.Net.Http.Headers;

namespace OnlineCinemaFestival.Client.Services;

public class ManipuladorTokenHttp : DelegatingHandler
{
    private readonly ArmazenamentoToken _armazenamento;

    public ManipuladorTokenHttp(ArmazenamentoToken armazenamento)
    {
        _armazenamento = armazenamento;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _armazenamento.ObterAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
