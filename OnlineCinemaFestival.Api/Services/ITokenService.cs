using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface ITokenService
{
    string CreateToken(Utilizador utilizador);
}
