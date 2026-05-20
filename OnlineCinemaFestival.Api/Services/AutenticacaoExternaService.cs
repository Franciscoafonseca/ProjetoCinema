using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public class AutenticacaoExternaService : IAutenticacaoExternaService
{
    private static readonly string[] ProvedoresSuportados = ["Google", "Apple"];
    private readonly IConfiguration _configuration;

    public AutenticacaoExternaService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<List<ProvedorAutenticacaoExternaDTO>> ObterProvedoresAsync()
    {
        var provedores = ProvedoresSuportados
            .Select(provider => new ProvedorAutenticacaoExternaDTO
            {
                Provider = provider,
                Nome = provider,
                Configurado = EstaConfigurado(provider),
            })
            .ToList();

        return Task.FromResult(provedores);
    }

    public Task<AutenticacaoRespostaDTO> AutenticarAsync(PedidoAutenticacaoExternaDTO request)
    {
        var provider = NormalizarProvider(request.Provider);

        if (!EstaConfigurado(provider))
        {
            throw new InvalidOperationException(
                $"Autenticacao com {provider} ainda nao esta configurada no servidor."
            );
        }

        throw new NotSupportedException(
            $"A validacao OAuth de {provider} esta preparada, mas ainda nao foi implementada."
        );
    }

    private static string NormalizarProvider(string provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Provider externo obrigatorio.");

        var normalizado = ProvedoresSuportados.FirstOrDefault(p =>
            p.Equals(provider.Trim(), StringComparison.OrdinalIgnoreCase)
        );

        if (normalizado is null)
            throw new ArgumentException("Provider externo nao suportado.");

        return normalizado;
    }

    private bool EstaConfigurado(string provider)
    {
        var section = _configuration.GetSection($"AutenticacaoExterna:{provider}");
        return !string.IsNullOrWhiteSpace(section["ClientId"]);
    }
}
