using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class FestivalMapper
{
    public static FestivalReadDTO MapToReadDTO(Festival festival)
    {
        return new FestivalReadDTO
        {
            Id = festival.Id,
            Name = festival.Name,
            Description = festival.Description,
            StartDate = festival.StartDate,
            EndDate = festival.EndDate,
            Premios = festival.Premios,
            Filmes = festival.FestivalFilmes.Select(ff => (FilmeResumoDTO)FilmeMapper.MapToReadDTO(ff.Filme)).ToList(),
            Sessoes = festival.Sessoes.Select(s => (SessaoResumoDTO)SessaoMapper.MapToReadDTO(s)).ToList(),
            PassesDisponiveis = festival
                .Acessos.Where(a =>
                    a.IsAtivo
                    && (
                        a.Tipo == TipoAcesso.PasseDiario
                        || a.Tipo == TipoAcesso.PasseCompleto
                    )
                )
                .Select(AcessoMapper.MapToReadDTO)
                .ToList(),
        };
    }

    public static Festival MapFromCreateDTO(FestivalCreateDTO dto)
    {
        return new Festival
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
        };
    }

    public static void MapToExistingFestival(FestivalUpdateDTO dto, Festival festival)
    {
        festival.Name = dto.Name.Trim();
        festival.Description = dto.Description.Trim();
        festival.StartDate = dto.StartDate;
        festival.EndDate = dto.EndDate;
    }
}
