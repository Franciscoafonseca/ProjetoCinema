using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class ComentarioMapper
{
    // DTO (ComentarioCreateDTO) para Entity (BD)
    public static Comentario ToEntity(int comunidadeId, int utilizadorId, ComentarioCreateDTO dto)
    {
        return new Comentario
        {
            ComunidadeId = comunidadeId,
            FilmeId = dto.FilmeId,
            UsuarioId = utilizadorId,
            Texto = dto.Texto.Trim(),
            CriadoEm = DateTime.UtcNow,
            Reportado = false,
            Visivel = true,
        };
    }

    public static Comentario ToFilmeEntity(int filmeId, int utilizadorId, ComentarioCreateDTO dto)
    {
        return new Comentario
        {
            FilmeId = filmeId,
            UsuarioId = utilizadorId,
            Texto = dto.Texto.Trim(),
            CriadoEm = DateTime.UtcNow,
            Reportado = false,
            Visivel = true,
        };
    }

    // Entity (BD) para DTO (ComentarioReadDTO)
    public static ComentarioReadDTO ToReadDTO(Comentario comentario)
    {
        return new ComentarioReadDTO
        {
            Id = comentario.Id,
            UsuarioId = comentario.UsuarioId,
            NomeUsuario = comentario.Usuario.Name,
            UsuarioFotoUrl = comentario.Usuario.Perfil?.ProfileImageUrl,
            UsuarioPerfilPublico = comentario.Usuario.Perfil?.IsPublic ?? false,
            ComunidadeId = comentario.ComunidadeId,
            NomeComunidade = comentario.Comunidade?.Name,
            FilmeId = comentario.FilmeId,
            TituloFilme = comentario.Filme?.Titulo,
            FilmeCapaUrl = comentario.Filme?.CapaUrl,
            Texto = comentario.Texto,
            CriadoEm = comentario.CriadoEm,
            Visivel = comentario.Visivel,
            Reportado = comentario.Reportado,
        };
    }
}
