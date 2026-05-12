using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.AcessosFolder;

public class EstrategiaValidacaoPasseDiario : IEstrategiaValidacaoAcesso
{
    private readonly IFestivalRepository _festivalRepository;

    public EstrategiaValidacaoPasseDiario(IFestivalRepository festivalRepository)
    {
        _festivalRepository = festivalRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.PasseDiario;

    public string Nome => "Passe diário";

    public string Descricao => "Passe que dá acesso às sessões de um festival durante um dia.";

    public async Task ValidarAsync(AcessoCreateDto dto)
    {
        if (!dto.FestivalId.HasValue)
            throw new ArgumentException("Um passe diário precisa de FestivalId.");

        if (!dto.DataAcesso.HasValue)
            throw new ArgumentException("Um passe diário precisa de DataAcesso.");

        var festival = await _festivalRepository.GetByIdAsync(dto.FestivalId.Value);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        var data = dto.DataAcesso.Value.Date;

        if (data < festival.StartDate.Date || data > festival.EndDate.Date)
            throw new ArgumentException(
                "A data do passe diário deve estar dentro do período do festival."
            );
    }
}
