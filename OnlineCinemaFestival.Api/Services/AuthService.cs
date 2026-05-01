using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IPasswordHashingStrategy _passwordHashingStrategy;
    private readonly ITokenService _tokenService;

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

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var existingUser = await _utilizadorRepository.GetByEmailAsync(email);

        if (existingUser != null)
            throw new ArgumentException("Já existe um utilizador com este email.");

        var utilizador = new Utilizador
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = UserRole.User,
            IsActive = true,
            Nationality = request.Nationality.Trim(),
            CreatedAt = DateTime.UtcNow,
            Perfil = new PerfilUtilizador { IsPublic = true, CreatedAt = DateTime.UtcNow },
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

        utilizador.PasswordHash = _passwordHashingStrategy.HashPassword(
            utilizador,
            request.Password
        );

        await _utilizadorRepository.AddAsync(utilizador);

        var token = _tokenService.CreateToken(utilizador);

        return new AuthResponse
        {
            Token = token,
            UserId = utilizador.Id,
            Name = utilizador.Name,
            Email = utilizador.Email,
            Role = utilizador.Role.ToString(),
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var utilizador = await _utilizadorRepository.GetByEmailAsync(email);

        if (utilizador == null)
            throw new ArgumentException("Credenciais inválidas.");

        if (!utilizador.IsActive)
            throw new ArgumentException("Esta conta está inativa.");

        var validPassword = _passwordHashingStrategy.VerifyPassword(utilizador, request.Password);

        if (!validPassword)
            throw new ArgumentException("Credenciais inválidas.");

        utilizador.LastLoginAt = DateTime.UtcNow;

        await _utilizadorRepository.SaveChangesAsync();

        var token = _tokenService.CreateToken(utilizador);

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
