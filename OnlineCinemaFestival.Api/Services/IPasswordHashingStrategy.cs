using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IPasswordHashingStrategy
{
    string HashPassword(Utilizador utilizador, string password);

    bool VerifyPassword(Utilizador utilizador, string password);
}
