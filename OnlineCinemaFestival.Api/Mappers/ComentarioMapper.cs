using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class ComentarioMapper
{
    // DTO (ComentarioCreateDto) para Entity (BD)
    public static Comentario ToEntity(int comunidadeId, int usuarioId, ComentarioCreateDto dto)
    {
        return new Comentario
        {
            ComunidadeId = comunidadeId,
            UsuarioId = usuarioId,
            Texto = dto.Texto,
            CriadoEm = DateTime.UtcNow,
            Reportado = false,
            Visivel = true,
        };
    }

    // Entity (BD) para DTO (ComentarioReadDto)
    public static ComentarioReadDto ToReadDto(Comentario comentario)
    {
        return new ComentarioReadDto
        {
            Id = comentario.Id,
            UsuarioId = comentario.UsuarioId,
            NomeUsuario = comentario.Usuario.Name,
            ComunidadeId = comentario.ComunidadeId,
            NomeComunidade = comentario.Comunidade?.Name,
            FilmeId = comentario.FilmeId,
            TituloFilme = comentario.Filme?.Titulo,
            Texto = comentario.Texto,
            CriadoEm = comentario.CriadoEm,
            Visivel = comentario.Visivel,
            Reportado = comentario.Reportado,
        };
    }
}
