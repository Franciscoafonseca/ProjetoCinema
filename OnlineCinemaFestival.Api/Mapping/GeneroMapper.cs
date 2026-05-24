using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Mapping;

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
