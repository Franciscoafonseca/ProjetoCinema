using Microsoft.AspNetCore.Identity;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PasswordHashingStrategy : IPasswordHashingStrategy
{
    private readonly PasswordHasher<Utilizador> _passwordHasher = new();

    public string HashPassword(Utilizador utilizador, string password)
    {
        return _passwordHasher.HashPassword(utilizador, password);
    }

    public bool VerifyPassword(Utilizador utilizador, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            utilizador,
            utilizador.PasswordHash,
            password
        );

        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
