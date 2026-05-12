using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoUtilizadorService
{
    Task<IEnumerable<AcessoUtilizadorReadDto>> ObterAcessosDoUtilizadorAsync(int utilizadorId);
}
