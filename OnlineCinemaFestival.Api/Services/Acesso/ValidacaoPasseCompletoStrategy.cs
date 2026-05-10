using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.Acesso;

public class ValidacaoPasseCompletoStrategy : IEstrategiaValidacaoAcesso
{
    private readonly IFestivalRepository _festivalRepository;

    public ValidacaoPasseCompletoStrategy(IFestivalRepository festivalRepository)
    {
        _festivalRepository = festivalRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.PasseCompleto;

    public string Nome => "Passe completo";

    public string Descricao => "Passe que dá acesso a todas as sessões de um festival.";

    public async Task ValidarAsync(AcessoCreateDto dto)
    {
        if (!dto.FestivalId.HasValue)
            throw new ArgumentException("Um passe completo precisa de FestivalId.");

        var festival = await _festivalRepository.GetByIdAsync(dto.FestivalId.Value);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");
    }
}
