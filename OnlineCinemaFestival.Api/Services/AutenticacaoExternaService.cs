using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class AutenticacaoExternaService : IAutenticacaoExternaService
{
    private static readonly string[] ProvedoresSuportados = ["Google", "Apple"];
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IPasswordHashingStrategy _passwordHashingStrategy;
    private readonly ITokenService _tokenService;

    public AutenticacaoExternaService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IUtilizadorRepository utilizadorRepository,
        IPasswordHashingStrategy passwordHashingStrategy,
        ITokenService tokenService
    )
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _utilizadorRepository = utilizadorRepository;
        _passwordHashingStrategy = passwordHashingStrategy;
        _tokenService = tokenService;
    }

    public Task<List<ProvedorAutenticacaoExternaDTO>> ObterProvedoresAsync()
    {
        var provedores = ProvedoresSuportados
            .Select(provider => new ProvedorAutenticacaoExternaDTO
            {
                Provider = provider,
                Nome = provider,
                ClientId = ObterClientId(provider),
                Configurado = EstaConfigurado(provider),
            })
            .ToList();

        return Task.FromResult(provedores);
    }

    public async Task<AutenticacaoRespostaDTO> AutenticarAsync(
        PedidoAutenticacaoExternaDTO request
    )
    {
        var provider = NormalizarProvider(request.Provider);
        var clientId = ObterClientId(provider);

        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException(
                $"Autenticacao com {provider} ainda nao esta configurada no servidor."
            );

        var principal = provider switch
        {
            "Google" => await ValidarGoogleAsync(request, clientId),
            "Apple" => await ValidarAppleAsync(request, clientId),
            _ => throw new ArgumentException("Provider externo nao suportado."),
        };

        var email = ObterClaimObrigatoria(principal, ClaimTypes.Email, "email")
            .Trim()
            .ToLowerInvariant();
        var nome =
            ObterClaimOpcional(principal, ClaimTypes.Name, "name")
            ?? email.Split('@', StringSplitOptions.RemoveEmptyEntries)[0];
        var externalId =
            ObterClaimOpcional(principal, ClaimTypes.NameIdentifier, "sub")
            ?? throw new ArgumentException("Token externo sem identificador.");

        var utilizador = await _utilizadorRepository.ObterPorEmailAsync(email);

        if (utilizador == null)
        {
            utilizador = CriarUtilizadorExterno(provider, externalId, nome, email);
            await _utilizadorRepository.AddAsync(utilizador);
        }

        if (!utilizador.IsActive)
            throw new ArgumentException("Esta conta esta inativa.");

        utilizador.LastLoginAt = DateTime.UtcNow;
        await _utilizadorRepository.SaveChangesAsync();

        return new AutenticacaoRespostaDTO
        {
            Token = _tokenService.CreateToken(utilizador),
            UserId = utilizador.Id,
            Name = utilizador.Name,
            Email = utilizador.Email,
            Role = utilizador.Role.ToString(),
        };
    }

    private async Task<ClaimsPrincipal> ValidarGoogleAsync(
        PedidoAutenticacaoExternaDTO request,
        string clientId
    )
    {
        if (!string.IsNullOrWhiteSpace(request.IdToken))
        {
            return await ValidarJwtAsync(
                request.IdToken,
                clientId,
                ["https://accounts.google.com", "accounts.google.com"],
                "https://www.googleapis.com/oauth2/v3/certs"
            );
        }

        if (string.IsNullOrWhiteSpace(request.AccessToken))
            throw new ArgumentException("Token Google obrigatorio.");

        using var http = _httpClientFactory.CreateClient();
        using var message = new HttpRequestMessage(
            HttpMethod.Get,
            "https://www.googleapis.com/oauth2/v3/userinfo"
        );
        message.Headers.Authorization = new("Bearer", request.AccessToken);

        using var response = await http.SendAsync(message);
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException("Token Google invalido.");

        await using var stream = await response.Content.ReadAsStreamAsync();
        var userInfo = await JsonSerializer.DeserializeAsync<GoogleUserInfo>(stream, JsonOptions)
            ?? throw new ArgumentException("Resposta Google invalida.");

        if (!userInfo.EmailVerified)
            throw new ArgumentException("Email Google nao verificado.");

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim("sub", userInfo.Sub),
                    new Claim("email", userInfo.Email),
                    new Claim("name", userInfo.Name ?? userInfo.Email),
                ],
                "Google"
            )
        );
    }

    private async Task<ClaimsPrincipal> ValidarAppleAsync(
        PedidoAutenticacaoExternaDTO request,
        string clientId
    )
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            throw new ArgumentException("Id token Apple obrigatorio.");

        return await ValidarJwtAsync(
            request.IdToken,
            clientId,
            ["https://appleid.apple.com"],
            "https://appleid.apple.com/auth/keys"
        );
    }

    private async Task<ClaimsPrincipal> ValidarJwtAsync(
        string token,
        string audience,
        string[] issuers,
        string jwksUrl
    )
    {
        using var http = _httpClientFactory.CreateClient();
        var jwksJson = await http.GetStringAsync(jwksUrl);
        var keys = new JsonWebKeySet(jwksJson);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys.GetSigningKeys(),
            ValidateIssuer = true,
            ValidIssuers = issuers,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),
            NameClaimType = "name",
            RoleClaimType = ClaimTypes.Role,
        };

        return new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
    }

    private Utilizador CriarUtilizadorExterno(
        string provider,
        string externalId,
        string nome,
        string email
    )
    {
        var agora = DateTime.UtcNow;
        var utilizador = new Utilizador
        {
            Name = string.IsNullOrWhiteSpace(nome) ? email : nome.Trim(),
            Email = email,
            PhoneNumber = string.Empty,
            Role = PapelUtilizador.Utilizador,
            IsActive = true,
            Nationality = "PT",
            CreatedAt = agora,
            Perfil = new PerfilUtilizador
            {
                Nationality = "Portugal",
                CountryCode = "PT",
                Location = "Lisboa",
                IsPublic = true,
                CreatedAt = agora,
            },
            ListasPessoais = CriarListasPadrao(agora),
        };

        utilizador.PasswordHash = _passwordHashingStrategy.HashPassword(
            utilizador,
            $"{provider}:{externalId}:{Guid.NewGuid():N}"
        );

        return utilizador;
    }

    private static List<ListaPessoal> CriarListasPadrao(DateTime agora) =>
    [
        new()
        {
            Name = "Quero ver",
            Tipo = TipoListaPessoal.Watchlist,
            IsPublic = false,
            CreatedAt = agora,
        },
        new()
        {
            Name = "Vistos",
            Tipo = TipoListaPessoal.Watched,
            IsPublic = false,
            CreatedAt = agora,
        },
        new()
        {
            Name = "Favoritos",
            Tipo = TipoListaPessoal.Favorites,
            IsPublic = false,
            CreatedAt = agora,
        },
    ];

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

    private string ObterClientId(string provider) =>
        _configuration[$"AutenticacaoExterna:{provider}:ClientId"] ?? string.Empty;

    private bool EstaConfigurado(string provider) => !string.IsNullOrWhiteSpace(ObterClientId(provider));

    private static string ObterClaimObrigatoria(
        ClaimsPrincipal principal,
        params string[] nomes
    ) =>
        ObterClaimOpcional(principal, nomes)
        ?? throw new ArgumentException("Token externo sem email.");

    private static string? ObterClaimOpcional(ClaimsPrincipal principal, params string[] nomes)
    {
        foreach (var nome in nomes)
        {
            var valor = principal.FindFirstValue(nome);
            if (!string.IsNullOrWhiteSpace(valor))
                return valor;
        }

        return null;
    }

    private sealed class GoogleUserInfo
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
