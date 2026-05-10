using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class FestivalMapper
{
    public static FestivalReadDto MapToReadDto(Festival festival)
    {
        return new FestivalReadDto
        {
            Id = festival.Id,
            Name = festival.Name,
            Description = festival.Description,
            StartDate = festival.StartDate,
            EndDate = festival.EndDate,
        };
    }

    public static Festival MapFromCreateDto(FestivalCreateDto dto)
    {
        return new Festival
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
        };
    }

    public static void MapToExistingFestival(FestivalUpdateDto dto, Festival festival)
    {
        festival.Name = dto.Name.Trim();
        festival.Description = dto.Description.Trim();
        festival.StartDate = dto.StartDate;
        festival.EndDate = dto.EndDate;
    }
}
