using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.AcessosFolder;

public class ValidacaoPasseCompletoStrategy : IEstrategiaValidacaoAcesso
{
    private readonly IFestivalRepository _festivalRepository;

    public ValidacaoPasseCompletoStrategy(IFestivalRepository festivalRepository)
    {
        _festivalRepository = festivalRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.PasseCompleto;

    public string Nome => "Passe completo";

    public string Descricao => "Passe que da acesso a todas as sessoes de um festival.";

    public async Task ValidarAsync(AcessoCreateDTO dto)
    {
        if (!dto.FestivalId.HasValue)
            throw new ArgumentException("Um passe completo precisa de FestivalId.");

        if (
            dto.SessaoId.HasValue
            || dto.FilmeId.HasValue
            || dto.DataAcesso.HasValue
            || dto.DuracaoHoras.HasValue
        )
            throw new ArgumentException("Passe completo deve indicar apenas FestivalId como alvo.");

        var festival = await _festivalRepository.ObterPorIdAsync(dto.FestivalId.Value);

        if (festival == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        if (festival.EndDate < DateTime.UtcNow)
            throw new ArgumentException("O festival ja terminou.");
    }
}
