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
            UsuarioId = utilizadorId,
            Texto = dto.Texto,
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
