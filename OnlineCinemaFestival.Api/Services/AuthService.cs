using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Serviço responsável pela autenticação de utilizadores,
/// incluindo o registo, login, criação de tokens e validação de credenciais.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IPasswordHashingStrategy _passwordHashingStrategy;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Inicializa uma nova instância do serviço de autenticação.
    /// </summary>
    /// <param name="utilizadorRepository">
    /// Repositório responsável pelo acesso aos dados dos utilizadores.
    /// </param>
    /// <param name="passwordHashingStrategy">
    /// Estratégia utilizada para gerar e validar hashes de palavras-passe.
    /// </param>
    /// <param name="tokenService">
    /// Serviço responsável pela criação de tokens de autenticação.
    /// </param>
    public AuthService(
        IUtilizadorRepository utilizadorRepository,
        IPasswordHashingStrategy passwordHashingStrategy,
        ITokenService tokenService
    )
    {
        _utilizadorRepository = utilizadorRepository;
        _passwordHashingStrategy = passwordHashingStrategy;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Regista um novo utilizador no sistema.
    /// </summary>
    /// <param name="request">
    /// Dados necessários para o registo, incluindo nome, email, nacionalidade e palavra-passe.
    /// </param>
    /// <returns>
    /// Resposta de autenticação contendo o token e os dados principais do utilizador registado.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Lançada quando já existe um utilizador registado com o mesmo email.
    /// </exception>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Normaliza o email para evitar duplicações causadas por maiúsculas, minúsculas ou espaços.
        var email = request.Email.Trim().ToLower();

        // Verifica se já existe um utilizador com o email indicado.
        var existingUser = await _utilizadorRepository.GetByEmailAsync(email);

        if (existingUser != null)
            throw new ArgumentException("Já existe um utilizador com este email.");

        // Cria o novo utilizador com os dados fornecidos e valores padrão da aplicação.
        var utilizador = new Utilizador
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = UserRole.User,
            IsActive = true,
            Nationality = request.Nationality.Trim(),
            CreatedAt = DateTime.UtcNow,

            // Cria automaticamente um perfil público associado ao novo utilizador.
            Perfil = new PerfilUtilizador { IsPublic = true, CreatedAt = DateTime.UtcNow },

            // Cria automaticamente as listas pessoais padrão do utilizador.
            ListasPessoais = new List<ListaPessoal>
            {
                new()
                {
                    Name = "Quero ver",
                    Tipo = TipoListaPessoal.Watchlist,
                    IsPublic = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new()
                {
                    Name = "Vistos",
                    Tipo = TipoListaPessoal.Watched,
                    IsPublic = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new()
                {
                    Name = "Favoritos",
                    Tipo = TipoListaPessoal.Favorites,
                    IsPublic = false,
                    CreatedAt = DateTime.UtcNow,
                },
            },
        };

        // Gera o hash da palavra-passe antes de guardar o utilizador na base de dados.
        utilizador.PasswordHash = _passwordHashingStrategy.HashPassword(
            utilizador,
            request.Password
        );

        // Guarda o novo utilizador no repositório.
        await _utilizadorRepository.AddAsync(utilizador);

        // Cria um token de autenticação para o utilizador registado.
        var token = _tokenService.CreateToken(utilizador);

        // Devolve os dados necessários para autenticar o utilizador no frontend.
        return new AuthResponse
        {
            Token = token,
            UserId = utilizador.Id,
            Name = utilizador.Name,
            Email = utilizador.Email,
            Role = utilizador.Role.ToString(),
        };
    }

    /// <summary>
    /// Autentica um utilizador existente no sistema.
    /// </summary>
    /// <param name="request">
    /// Dados de login, incluindo email e palavra-passe.
    /// </param>
    /// <returns>
    /// Resposta de autenticação contendo o token e os dados principais do utilizador autenticado.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Lançada quando as credenciais são inválidas ou a conta se encontra inativa.
    /// </exception>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Normaliza o email para garantir uma pesquisa consistente.
        var email = request.Email.Trim().ToLower();

        // Procura o utilizador associado ao email indicado.
        var utilizador = await _utilizadorRepository.GetByEmailAsync(email);

        if (utilizador == null)
            throw new ArgumentException("Credenciais inválidas.");

        // Impede o login de contas desativadas.
        if (!utilizador.IsActive)
            throw new ArgumentException("Esta conta está inativa.");

        // Verifica se a palavra-passe introduzida corresponde ao hash guardado.
        var validPassword = _passwordHashingStrategy.VerifyPassword(utilizador, request.Password);

        if (!validPassword)
            throw new ArgumentException("Credenciais inválidas.");

        // Atualiza a data do último login do utilizador.
        utilizador.LastLoginAt = DateTime.UtcNow;

        // Guarda a alteração da data de último login.
        await _utilizadorRepository.SaveChangesAsync();

        // Cria um novo token de autenticação.
        var token = _tokenService.CreateToken(utilizador);

        // Devolve os dados necessários para manter a sessão autenticada.
        return new AuthResponse
        {
            Token = token,
            UserId = utilizador.Id,
            Name = utilizador.Name,
            Email = utilizador.Email,
            Role = utilizador.Role.ToString(),
        };
    }
}
