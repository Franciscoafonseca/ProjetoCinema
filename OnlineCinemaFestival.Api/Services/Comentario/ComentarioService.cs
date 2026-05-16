using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IComunidadeRepository _comunidadeRepository;

    public ComentarioService(
        IComentarioRepository comentarioRepository,
        IUtilizadorRepository utilizadorRepository,
        IComunidadeRepository comunidadeRepository
    )
    {
        _comentarioRepository = comentarioRepository;
        _utilizadorRepository = utilizadorRepository;
        _comunidadeRepository = comunidadeRepository;
    }

    public async Task<ComentarioReadDTO> CriarComentarioAsync(
        int comunidadeId,
        ComentarioCreateDTO dto,
        int usuarioId
    )
    {
        var comunidade = await _comunidadeRepository.ObterComunidadePorIdAsync(comunidadeId);
        if (comunidade == null)
            throw new Exception("Comunidade não encontrada");

        if (!comunidade.IsPublic)
        {
            var isMembro = await _comunidadeRepository.IsMembroAsync(comunidadeId, usuarioId);
            if (!isMembro)
                throw new UnauthorizedAccessException("Acesso negado à comunidade privada");
        }

        var usuario = await _utilizadorRepository.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            throw new Exception("Usuário não encontrado");

        var comentario = ComentarioMapper.ToEntity(comunidadeId, usuarioId, dto);
        var result = await _comentarioRepository.AddAsync(comentario);

        result.Usuario = usuario; // Para preencher o NomeUsuario no DTO
        result.Comunidade = comunidade; // Para preencher o NomeComunidade no DTO
        return ComentarioMapper.ToReadDTO(result);
    }

    public async Task<IEnumerable<ComentarioReadDTO>> ObterComentariosPorComunidadeIdAsync(
        int comunidadeId
    )
    {
        var listaDeComentarios = await _comentarioRepository.ObterPorComunidadeIdAsync(comunidadeId);
        return listaDeComentarios.Select(ComentarioMapper.ToReadDTO);
    }
}
