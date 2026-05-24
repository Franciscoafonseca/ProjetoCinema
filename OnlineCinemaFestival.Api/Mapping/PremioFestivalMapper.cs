using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Mapping;

public static class PremioFestivalMapper
{
    public static PremioFestivalReadDTO MapToReadDTO(PremioFestival premio)
    {
        return MapToReadDTO(premio, DateTime.UtcNow);
    }

    public static PremioFestivalReadDTO MapToReadDTO(PremioFestival premio, DateTime agoraUtc)
    {
        return new PremioFestivalReadDTO
        {
            Id = premio.Id,
            FestivalId = premio.FestivalId,
            Nome = premio.Nome,
            Descricao = premio.Descricao,
            DataAberturaVotacao = premio.DataAberturaVotacao,
            DataFechoVotacao = premio.DataFechoVotacao,
            EstadoPremio = ObterEstadoVotacao(premio, agoraUtc),
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

    private static string ObterEstadoVotacao(PremioFestival premio, DateTime agoraUtc)
    {
        if (premio.EstadoPremio == EstadoPremio.Publicado || premio.Resultado != null)
            return "Vencedor publicado";

        if (agoraUtc < premio.DataAberturaVotacao)
            return "Nao iniciada";

        if (agoraUtc <= premio.DataFechoVotacao)
            return "Aberta";

        return "Encerrada";
    }
}
