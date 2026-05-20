using System.Net;
using System.Text.Json;

namespace OnlineCinemaFestival.Client.Services;

public static class MensagemErroApi
{
    public static async Task<string> ObterAsync(HttpResponseMessage resposta, string mensagemPadrao)
    {
        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
            return "Inicia sessao para continuar.";

        if (resposta.StatusCode == HttpStatusCode.Forbidden)
            return "Nao tens permissao para esta operacao.";

        var conteudo = await resposta.Content.ReadAsStringAsync();
        return Limpar(conteudo, mensagemPadrao);
    }

    public static string Limpar(string? mensagem, string mensagemPadrao)
    {
        if (string.IsNullOrWhiteSpace(mensagem))
            return mensagemPadrao;

        var valor = mensagem.Trim();

        try
        {
            using var documento = JsonDocument.Parse(valor);
            var raiz = documento.RootElement;

            if (raiz.ValueKind == JsonValueKind.String)
                return LimparTexto(raiz.GetString(), mensagemPadrao);

            if (raiz.TryGetProperty("message", out var message))
                return LimparTexto(message.GetString(), mensagemPadrao);

            if (raiz.TryGetProperty("title", out var title))
                return LimparTexto(title.GetString(), mensagemPadrao);

            if (raiz.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                foreach (var erro in errors.EnumerateObject())
                {
                    if (erro.Value.ValueKind != JsonValueKind.Array)
                        continue;

                    var primeiraMensagem = erro.Value
                        .EnumerateArray()
                        .Select(e => e.GetString())
                        .FirstOrDefault(e => !string.IsNullOrWhiteSpace(e));

                    if (!string.IsNullOrWhiteSpace(primeiraMensagem))
                        return LimparTexto(primeiraMensagem, mensagemPadrao);
                }
            }
        }
        catch (JsonException)
        {
            return LimparTexto(valor, mensagemPadrao);
        }

        return mensagemPadrao;
    }

    private static string LimparTexto(string? texto, string mensagemPadrao)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return mensagemPadrao;

        return texto.Trim().Trim('"');
    }
}
