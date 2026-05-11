using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Mappers;

namespace OnlineCinemaFestival.Api.Services;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IUtilizadorRepository _utilizadorRepository;

    public ComentarioService(IComentarioRepository comentarioRepository, IUtilizadorRepository utilizadorRepository)
    {
        _comentarioRepository = comentarioRepository;
        _utilizadorRepository = utilizadorRepository;
    }

    public async Task<ComentarioReadDto> CriarComentarioAsync(int comunidadeId, ComentarioCreateDto dto, int usuarioId)
    {
        var usuario = await _utilizadorRepository.GetByIdAsync(usuarioId);
        if (usuario == null) throw new Exception("Usuário não encontrado");

        var comentario = ComentarioMapper.ToEntity(comunidadeId, usuarioId, dto);
        var result = await _comentarioRepository.AddAsync(comentario);

        result.Usuario = usuario; // Para preencher o NomeUsuario no DTO
        return ComentarioMapper.ToReadDto(result);
    }

    public async Task<IEnumerable<ComentarioReadDto>> ObterComentariosPorComunidadeIdAsync(int comunidadeId)
    {
        var listaDeComentarios = await _comentarioRepository.GetByComunidadeIdAsync(comunidadeId);
        return listaDeComentarios.Select(ComentarioMapper.ToReadDto);
    }


}