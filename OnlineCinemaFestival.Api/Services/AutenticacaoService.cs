using System.Text.RegularExpressions;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IPasswordHashingStrategy _passwordHashingStrategy;
    private readonly ITokenService _tokenService;

    public AutenticacaoService(
        IUtilizadorRepository utilizadorRepository,
        IPasswordHashingStrategy passwordHashingStrategy,
        ITokenService tokenService
    )
    {
        _utilizadorRepository = utilizadorRepository;
        _passwordHashingStrategy = passwordHashingStrategy;
        _tokenService = tokenService;
    }

    public async Task<AutenticacaoRespostaDTO> RegistarAsync(PedidoRegistoDTO request)
    {
        ValidarRegisto(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var telefone = NormalizarTelefone(request.PhoneNumber);
        var pais = PerfilOpcoes.ObterPaisValido(
            string.IsNullOrWhiteSpace(request.CountryCode) ? request.Nationality : request.CountryCode
        );
        var localidade = PerfilOpcoes.ObterLocalidadeValida(request.Location);

        var utilizadorExistente = await _utilizadorRepository.ObterPorEmailAsync(email);

        if (utilizadorExistente != null)
            throw new ArgumentException("Ja existe um utilizador com este email.");

        var telefoneExistente = await _utilizadorRepository.ObterPorTelefoneAsync(telefone);

        if (telefoneExistente != null)
            throw new ArgumentException("Ja existe um utilizador com este telefone.");

        var utilizador = new Utilizador
        {
            Name = request.Name.Trim(),
            Email = email,
            PhoneNumber = telefone,
            Role = PapelUtilizador.Utilizador,
            IsActive = true,
            Nationality = pais.Codigo,
            CreatedAt = DateTime.UtcNow,
            Perfil = new PerfilUtilizador
            {
                Nationality = pais.Nome,
                CountryCode = pais.Codigo,
                Location = localidade,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow,
            },
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

        return CriarResposta(utilizador);
    }

    public async Task<AutenticacaoRespostaDTO> EntrarAsync(PedidoLoginDTO request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var utilizador = await _utilizadorRepository.ObterPorEmailAsync(email);

        if (utilizador == null)
            throw new ArgumentException("Credenciais invalidas.");

        if (!utilizador.IsActive)
            throw new ArgumentException("Esta conta esta inativa.");

        var validPassword = _passwordHashingStrategy.VerifyPassword(utilizador, request.Password);

        if (!validPassword)
            throw new ArgumentException("Credenciais invalidas.");

        utilizador.LastLoginAt = DateTime.UtcNow;

        await _utilizadorRepository.SaveChangesAsync();

        return CriarResposta(utilizador);
    }

    private AutenticacaoRespostaDTO CriarResposta(Utilizador utilizador)
    {
        var token = _tokenService.CreateToken(utilizador);

        return new AutenticacaoRespostaDTO
        {
            Token = token,
            UserId = utilizador.Id,
            Name = utilizador.Name,
            Email = utilizador.Email,
            Role = utilizador.Role.ToString(),
        };
    }

    private static void ValidarRegisto(PedidoRegistoDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("O nome e obrigatorio.");

        if (request.Name.Trim().Length > 120)
            throw new ArgumentException("O nome nao pode exceder 120 caracteres.");

        _ = NormalizarTelefone(request.PhoneNumber);

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
            throw new ArgumentException("A confirmacao da palavra-passe nao coincide.");

        ValidarPasswordForte(request.Password);
    }

    private static void ValidarPasswordForte(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException("A palavra-passe deve ter pelo menos 8 caracteres.");

        if (!password.Any(char.IsUpper)
            || !password.Any(char.IsLower)
            || !password.Any(char.IsDigit)
            || !password.Any(c => !char.IsLetterOrDigit(c)))
        {
            throw new ArgumentException(
                "A palavra-passe deve incluir maiusculas, minusculas, numeros e simbolos."
            );
        }
    }

    private static string NormalizarTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("O telefone e obrigatorio.");

        var normalizado = Regex.Replace(telefone.Trim(), @"[\s().-]", "");

        if (!Regex.IsMatch(normalizado, @"^\+?[0-9]{7,15}$"))
            throw new ArgumentException("Introduz um telefone valido.");

        return normalizado;
    }
}
