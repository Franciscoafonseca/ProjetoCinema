using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class GeneroMapper
{
    public static GeneroDto MapToDto(Genero genero)
    {
        return new GeneroDto { Id = genero.Id, Name = genero.Name };
    }

    public static Genero MapFromCreateDto(CriarGeneroDto dto)
    {
        return new Genero { Name = dto.Name.Trim() };
    }
}
