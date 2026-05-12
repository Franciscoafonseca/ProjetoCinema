using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class ComunidadeMapper
{
    // From Dto (ComunidadeCreateDto) to Entity (BD)
    public static Comunidade ToEntity(ComunidadeCreateDto dto, int criatedByUserId)
    {
        return new Comunidade
        {
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            IsPublic = dto.IsPublic,
            CreatedByUserId = criatedByUserId,
            CreatedAt = DateTime.UtcNow
        };
    }

    // From Entity (BD) to Dto (ComunidadeReadDto)
    public static ComunidadeReadDto ToReadDto(Comunidade comunidade)
    {
        return new ComunidadeReadDto
        {
            Id = comunidade.Id,
            Name = comunidade.Name,
            Description = comunidade.Description,
            ImageUrl = comunidade.ImageUrl,
            IsPublic = comunidade.IsPublic,
            CreatedByUserId = comunidade.CreatedByUserId,
            CreatedByUserName = comunidade.CreatedByUser?.Name,
            CreatedAt = comunidade.CreatedAt,
            MembersCount = comunidade.Members?.Count ?? 0,
            ComentariosCount = comunidade.Comentarios?.Count ?? 0
        };
    }


}