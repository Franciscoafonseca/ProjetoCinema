using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoUtilizadorService
{
    Task<IEnumerable<AcessoUtilizadorReadDto>> ObterAcessosDoUtilizadorAsync(int utilizadorId);

    Task<IEnumerable<AcessoUtilizadorReadDto>> ObterAcessosAtivosDoUtilizadorAsync(
        int utilizadorId
    );

    Task<bool> UtilizadorTemAcessoAFilmeAsync(int utilizadorId, int filmeId, int? festivalId);

    Task<bool> UtilizadorTemAcessoASessaoAsync(int utilizadorId, int sessaoId);
}
