using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface ITokenService
{
    string CreateToken(Utilizador utilizador);
}
