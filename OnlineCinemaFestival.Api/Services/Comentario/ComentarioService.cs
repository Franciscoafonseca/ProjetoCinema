using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IComunidadeRepository _comunidadeRepository;
    private readonly IFilmeRepository _filmeRepository;

    public ComentarioService(
        IComentarioRepository comentarioRepository,
        IUtilizadorRepository utilizadorRepository,
        IComunidadeRepository comunidadeRepository,
        IFilmeRepository filmeRepository
    )
    {
        _comentarioRepository = comentarioRepository;
        _utilizadorRepository = utilizadorRepository;
        _comunidadeRepository = comunidadeRepository;
        _filmeRepository = filmeRepository;
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

    public async Task<ComentarioReadDTO> CriarComentarioFilmeAsync(
        int filmeId,
        ComentarioCreateDTO dto,
        int utilizadorId
    )
    {
        var filme = await _filmeRepository.ObterPorIdAsync(filmeId);
        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);
        if (utilizador == null)
            throw new KeyNotFoundException("Usuario nao encontrado.");

        if (!await _filmeRepository.UtilizadorViuFilmeAsync(utilizadorId, filmeId))
            throw new UnauthorizedAccessException("So podes comentar depois de ver o filme.");

        var comentario = ComentarioMapper.ToFilmeEntity(filmeId, utilizadorId, dto);
        var result = await _comentarioRepository.AddAsync(comentario);

        result.Usuario = utilizador;
        result.Filme = filme;

        return ComentarioMapper.ToReadDTO(result);
    }

    public async Task<IEnumerable<ComentarioReadDTO>> ObterComentariosPorFilmeIdAsync(int filmeId)
    {
        var filme = await _filmeRepository.ObterPorIdAsync(filmeId);
        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        var comentarios = await _comentarioRepository.ObterPorFilmeIdAsync(filmeId);
        return comentarios.Select(ComentarioMapper.ToReadDTO);
    }
}
