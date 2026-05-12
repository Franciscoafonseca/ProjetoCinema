using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoUtilizadorService : IAcessoUtilizadorService
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;

    public AcessoUtilizadorService(IAcessoUtilizadorRepository acessoUtilizadorRepository)
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
    }

    public async Task<IEnumerable<AcessoUtilizadorReadDto>> ObterAcessosDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var acessos = await _acessoUtilizadorRepository.GetByUtilizadorIdAsync(utilizadorId);

        return acessos.Select(AcessoUtilizadorMapper.MapToReadDto);
    }
}
