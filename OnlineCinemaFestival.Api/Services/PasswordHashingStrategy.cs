using Microsoft.AspNetCore.Identity;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Estratégia responsável por gerar e validar hashes de palavras-passe.
/// Utiliza o PasswordHasher do ASP.NET Core Identity para garantir maior segurança.
/// </summary>
public class PasswordHashingStrategy : IPasswordHashingStrategy
{
    private readonly PasswordHasher<Utilizador> _passwordHasher = new();

    /// <summary>
    /// Gera o hash da palavra-passe de um utilizador.
    /// </summary>
    /// <param name="utilizador">Utilizador associado à palavra-passe.</param>
    /// <param name="password">Palavra-passe em texto simples.</param>
    /// <returns>Hash seguro da palavra-passe.</returns>
    public string HashPassword(Utilizador utilizador, string password)
    {
        // Gera um hash seguro da palavra-passe antes de a guardar na base de dados.
        return _passwordHasher.HashPassword(utilizador, password);
    }

    /// <summary>
    /// Verifica se a palavra-passe introduzida corresponde ao hash guardado.
    /// </summary>
    /// <param name="utilizador">Utilizador cujas credenciais estão a ser validadas.</param>
    /// <param name="password">Palavra-passe introduzida no Entrar.</param>
    /// <returns>Verdadeiro se a palavra-passe for válida; caso contrário, falso.</returns>
    public bool VerifyPassword(Utilizador utilizador, string password)
    {
        // Compara a palavra-passe introduzida com o hash guardado no utilizador.
        var result = _passwordHasher.VerifyHashedPassword(
            utilizador,
            utilizador.PasswordHash,
            password
        );

        // Considera válida uma palavra-passe correta, mesmo que o hash necessite ser atualizado.
        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
