using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Serviço responsável pela criação de tokens JWT utilizados na autenticação.
/// O token contém informações essenciais do utilizador, como id, nome, email e role.
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    /// <summary>
    /// Inicializa uma nova instância do serviço de geração de tokens JWT.
    /// </summary>
    /// <param name="configuration">
    /// Configuração da aplicação, usada para obter os dados necessários à criação do token.
    /// </param>
    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Cria um token JWT para um utilizador autenticado.
    /// </summary>
    /// <param name="utilizador">Utilizador para o qual o token será gerado.</param>
    /// <returns>Token JWT em formato string.</returns>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando a chave JWT não está configurada.
    /// </exception>
    public string CreateToken(Utilizador utilizador)
    {
        var key = _options.Key;

        // Garante que existe uma chave configurada para assinar o token.
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("JWT Key não configurada.");

        // Cria a chave de segurança usada para assinar digitalmente o token.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        // Define o algoritmo de assinatura do token.
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Define as informações do utilizador que serão incluídas no token.
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, utilizador.Id.ToString()),
            new(ClaimTypes.Name, utilizador.Name),
            new(ClaimTypes.Email, utilizador.Email),
            new(ClaimTypes.Role, utilizador.Role.ToString()),
        };

        // Cria o token JWT com emissor, audiência, claims, tempo de expiração e credenciais.
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
            signingCredentials: credentials
        );

        // Converte o token para string, para ser enviado ao frontend.
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
