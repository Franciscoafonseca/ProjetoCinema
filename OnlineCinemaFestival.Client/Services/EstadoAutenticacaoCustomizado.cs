using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace OnlineCinemaFestival.Client.Services;

public class EstadoAutenticacaoCustomizado : AuthenticationStateProvider
{
    private readonly ArmazenamentoToken _armazenamento;

    public EstadoAutenticacaoCustomizado(ArmazenamentoToken armazenamento)
    {
        _armazenamento = armazenamento;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _armazenamento.ObterAsync();

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var claims = ExtrairClaimsDoJwt(token);

        if (claims == null)
        {
            await _armazenamento.RemoverAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identidade = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identidade));
    }

    public void NotificarAutenticado()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotificarTerminouSessao()
    {
        var anonimo = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        NotifyAuthenticationStateChanged(Task.FromResult(anonimo));
    }

    private static IEnumerable<Claim>? ExtrairClaimsDoJwt(string token)
    {
        try
        {
            var partes = token.Split('.');

            if (partes.Length < 2)
                return null;

            var payload = DescodificarBase64Url(partes[1]);
            var json = Encoding.UTF8.GetString(payload);
            var dados = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if (dados == null)
                return null;

            // Validar expiração (claim "exp" em segundos epoch)
            if (dados.TryGetValue("exp", out var exp) && exp.TryGetInt64(out var segundos))
            {
                var expiracao = DateTimeOffset.FromUnixTimeSeconds(segundos);
                if (expiracao <= DateTimeOffset.UtcNow)
                    return null;
            }

            var claims = new List<Claim>();

            foreach (var (chave, valor) in dados)
            {
                if (valor.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in valor.EnumerateArray())
                        claims.Add(new Claim(chave, item.ToString()));
                }
                else
                {
                    claims.Add(new Claim(chave, valor.ToString()));
                }
            }

            return claims;
        }
        catch
        {
            return null;
        }
    }

    private static byte[] DescodificarBase64Url(string entrada)
    {
        var s = entrada.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2:
                s += "==";
                break;
            case 3:
                s += "=";
                break;
        }
        return Convert.FromBase64String(s);
    }
}
