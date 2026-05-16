using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class GeneroMapper
{
    public static GeneroDTO MapToDTO(Genero genero)
    {
        return new GeneroDTO { Id = genero.Id, Name = genero.Name };
    }

    public static Genero MapFromCreateDTO(CriarGeneroDTO dto)
    {
        return new Genero { Name = dto.Name.Trim() };
    }
}
