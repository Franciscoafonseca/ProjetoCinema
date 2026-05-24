using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class ComunidadeMapper
{
    // DTO (ComunidadeCreateDTO) para entidade (BD).
    public static Comunidade ToEntity(ComunidadeCreateDTO dto, int criadoPorUtilizadorId)
    {
        return new Comunidade
        {
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            IsPublic = dto.IsPublic,
            CreatedByUserId = criadoPorUtilizadorId,
            CreatedAt = DateTime.UtcNow,
        };
    }

    // Entidade (BD) para DTO (ComunidadeReadDTO).
    public static ComunidadeReadDTO ToReadDTO(Comunidade comunidade, int utilizadorIdPedido)
    {
        return new ComunidadeReadDTO
        {
            PublicId = comunidade.PublicId,
            IsOwner = comunidade.Members?.Any(m =>
                m.UtilizadorId == utilizadorIdPedido
                && m.Role == PapelMembroComunidade.Proprietario
            ) ?? false,
            Name = comunidade.Name,
            Description = comunidade.Description,
            ImageUrl = comunidade.ImageUrl,
            IsPublic = comunidade.IsPublic,
            CodigoConvite = comunidade.CodigoConvite,
            CreatedByUserId = comunidade.CreatedByUserId,
            CreatedByUserName = comunidade.CreatedByUser?.Name,
            CreatedAt = comunidade.CreatedAt,
            MembersCount = comunidade.Members?.Count ?? 0,
            ComentariosCount = comunidade.Comentarios?.Count(c => c.Visivel) ?? 0,
            Members = (comunidade.Members ?? new List<ComunidadeMembro>())
                .OrderBy(m => m.JoinedAt)
                .Select(m => new MembroComunidadeRespostaDTO
                {
                    UserId = m.UtilizadorId,
                    Name = m.Utilizador?.Name ?? "Utilizador",
                    ProfileImageUrl = m.Utilizador?.Perfil?.ProfileImageUrl ?? string.Empty,
                    Role = m.Role.ToString(),
                    JoinedAt = m.JoinedAt,
                })
                .ToList(),
        };
    }
}
