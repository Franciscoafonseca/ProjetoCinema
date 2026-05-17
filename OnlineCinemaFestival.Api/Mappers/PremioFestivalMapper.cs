using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class PremioFestivalMapper
{
    public static PremioFestivalReadDTO MapToReadDTO(PremioFestival premio)
    {
        return new PremioFestivalReadDTO
        {
            Id = premio.Id,
            FestivalId = premio.FestivalId,
            Nome = premio.Nome,
            Descricao = premio.Descricao,
            DataAberturaVotacao = premio.DataAberturaVotacao,
            DataFechoVotacao = premio.DataFechoVotacao,
            EstadoPremio = premio.EstadoPremio.ToString(),
        };
    }

    public static ResultadoPremioFestivalDTO MapResultadoToDTO(ResultadoPremioFestival resultado)
    {
        return new ResultadoPremioFestivalDTO
        {
            PremioFestivalId = resultado.PremioFestivalId,
            NomePremio = resultado.PremioFestival?.Nome ?? string.Empty,
            FestivalId = resultado.PremioFestival?.FestivalId ?? 0,
            FestivalNome = resultado.PremioFestival?.Festival?.Name ?? string.Empty,
            FilmeIdVencedor = resultado.FilmeIdVencedor,
            TituloFilmeVencedor = resultado.FilmeVencedor?.Titulo ?? string.Empty,
            CapaUrlFilmeVencedor = resultado.FilmeVencedor?.CapaUrl ?? string.Empty,
            TotalVotos = resultado.TotalVotos,
            PublicadoEm = resultado.PublicadoEm,
            PublicadoPorUtilizadorId = resultado.PublicadoPorUtilizadorId,
        };
    }
}
