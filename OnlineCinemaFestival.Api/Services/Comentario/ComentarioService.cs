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
        Guid comunidadeId,
        ComentarioCreateDTO dto,
        int utilizadorId
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(comunidadeId);
        if (comunidade == null)
            throw new Exception("Comunidade não encontrada");

        if (!comunidade.IsPublic)
        {
            // uso o id interno para verificar se o usuario é membro
            var eMembro = await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizadorId);
            if (!eMembro)
                throw new UnauthorizedAccessException("Acesso negado à comunidade privada");
        }

        var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);
        if (utilizador == null)
            throw new Exception("Usuário não encontrado");

        var comentario = ComentarioMapper.ToEntity(comunidade.Id, utilizadorId, dto);
        var result = await _comentarioRepository.AddAsync(comentario);

        result.Usuario = utilizador;
        result.Comunidade = comunidade;
        return ComentarioMapper.ToReadDTO(result);
    }

    public async Task<IEnumerable<ComentarioReadDTO>> ObterComentariosPorComunidadeIdAsync(
        Guid comunidadeId,
        int utilizadorId
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(comunidadeId);
        if (comunidade == null)
            throw new Exception("Comunidade não encontrada");

        bool acessoProibido =
            !comunidade.IsPublic
            && !await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizadorId);
        if (acessoProibido)
            throw new UnauthorizedAccessException("Acesso negado à comunidade privada");

        var listaDeComentarios = await _comentarioRepository.ObterPorComunidadeIdAsync(
            comunidade.Id
        );
        return listaDeComentarios.Select(ComentarioMapper.ToReadDTO);
    }
}
