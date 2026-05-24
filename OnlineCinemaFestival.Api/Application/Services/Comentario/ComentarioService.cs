using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Mapping;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _comentarioRepository;
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IComunidadeRepository _comunidadeRepository;
    private readonly IFilmeRepository _filmeRepository;
    private readonly IEnumerable<IComentarioObserver> _comentarioObservers;

    public ComentarioService(
        IComentarioRepository comentarioRepository,
        IUtilizadorRepository utilizadorRepository,
        IComunidadeRepository comunidadeRepository,
        IFilmeRepository filmeRepository,
        IEnumerable<IComentarioObserver> comentarioObservers
    )
    {
        _comentarioRepository = comentarioRepository;
        _utilizadorRepository = utilizadorRepository;
        _comunidadeRepository = comunidadeRepository;
        _filmeRepository = filmeRepository;
        _comentarioObservers = comentarioObservers;
    }

    public async Task<ComentarioReadDTO> CriarComentarioAsync(
        Guid comunidadeId,
        ComentarioCreateDTO dto,
        int utilizadorId
    )
    {
        ValidarComentario(dto);

        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(comunidadeId);
        if (comunidade == null)
            throw new KeyNotFoundException("Comunidade nao encontrada.");

        if (!comunidade.IsPublic)
        {
            var eMembro = await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizadorId);
            if (!eMembro)
                throw new UnauthorizedAccessException("Acesso negado a comunidade privada.");
        }

        var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);
        if (utilizador == null)
            throw new KeyNotFoundException("Usuario nao encontrado.");

        Filme? filmeAssociado = null;
        if (dto.FilmeId.HasValue)
        {
            filmeAssociado = await _filmeRepository.ObterPorIdAsync(dto.FilmeId.Value);
            if (filmeAssociado == null)
                throw new KeyNotFoundException("Filme associado nao encontrado.");
        }

        var comentario = ComentarioMapper.ToEntity(comunidade.Id, utilizadorId, dto);
        var result = await _comentarioRepository.AddAsync(comentario);

        result.Usuario = utilizador;
        result.Comunidade = comunidade;
        result.Filme = filmeAssociado;
        await NotificarComentarioAsync(result);

        return ComentarioMapper.ToReadDTO(result);
    }

    public async Task<IEnumerable<ComentarioReadDTO>> ObterComentariosPorComunidadeIdAsync(
        Guid comunidadeId,
        int utilizadorId
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(comunidadeId);
        if (comunidade == null)
            throw new KeyNotFoundException("Comunidade nao encontrada.");

        var acessoProibido =
            !comunidade.IsPublic
            && !await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizadorId);
        if (acessoProibido)
            throw new UnauthorizedAccessException("Acesso negado a comunidade privada.");

        var listaDeComentarios = await _comentarioRepository.ObterPorComunidadeIdAsync(
            comunidade.Id,
            incluirModerados: await UtilizadorPodeModerarAsync(comunidade, utilizadorId)
        );
        return listaDeComentarios.Select(ComentarioMapper.ToReadDTO);
    }

    public async Task<ComentarioReadDTO> CriarComentarioFilmeAsync(
        int filmeId,
        ComentarioCreateDTO dto,
        int utilizadorId
    )
    {
        ValidarComentario(dto);

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
        await NotificarComentarioAsync(result);

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

    public async Task ReportarComentarioAsync(Guid comunidadeId, int comentarioId, int utilizadorId)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(comunidadeId);
        if (comunidade == null)
            throw new KeyNotFoundException("Comunidade nao encontrada.");

        var acessoProibido =
            !comunidade.IsPublic
            && !await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizadorId);
        if (acessoProibido)
            throw new UnauthorizedAccessException("Acesso negado a comunidade privada.");

        var comentario = await _comentarioRepository.GetByIdAsync(comentarioId);
        if (comentario == null || comentario.ComunidadeId != comunidade.Id)
            throw new KeyNotFoundException("Comentario nao encontrado.");

        if (!comentario.Reportado)
        {
            comentario.Reportado = true;
            await _comentarioRepository.UpdateAsync(comentario);
        }
    }

    public async Task<IEnumerable<ComentarioReadDTO>> ObterComentariosReportadosAsync(
        Guid comunidadeId,
        int utilizadorId
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(comunidadeId);
        if (comunidade == null)
            throw new KeyNotFoundException("Comunidade nao encontrada.");

        var isOwner = comunidade.Members.Any(m =>
            m.UtilizadorId == utilizadorId && m.Role == PapelMembroComunidade.Proprietario
        );

        if (!isOwner)
            throw new UnauthorizedAccessException("Apenas o proprietario pode ver reportes.");

        var comentarios = await _comentarioRepository.ObterReportadosPorComunidadeIdAsync(
            comunidade.Id
        );
        return comentarios.Select(ComentarioMapper.ToReadDTO);
    }

    public async Task AtualizarVisibilidadeComentarioAsync(
        Guid comunidadeId,
        int comentarioId,
        bool visivel,
        int utilizadorId
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(comunidadeId);
        if (comunidade == null)
            throw new KeyNotFoundException("Comunidade nao encontrada.");

        var isOwner = comunidade.Members.Any(m =>
            m.UtilizadorId == utilizadorId && m.Role == PapelMembroComunidade.Proprietario
        );

        if (!isOwner)
            throw new UnauthorizedAccessException(
                "Apenas o proprietario pode moderar comentarios."
            );

        var comentario = await _comentarioRepository.GetByIdAsync(comentarioId);
        if (comentario == null || comentario.ComunidadeId != comunidade.Id)
            throw new KeyNotFoundException("Comentario nao encontrado.");

        comentario.Visivel = visivel;
        comentario.EstadoModeracao = visivel
            ? EstadoModeracaoComentario.Visivel
            : EstadoModeracaoComentario.Oculto;
        await _comentarioRepository.UpdateAsync(comentario);
    }

    public async Task<ComentarioReadDTO> ModerarComentarioAsync(
        Guid comunidadeId,
        int comentarioId,
        ModerarComentarioDTO dto,
        int utilizadorId
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(comunidadeId);
        if (comunidade == null)
            throw new KeyNotFoundException("Comunidade nao encontrada.");

        if (!await UtilizadorPodeModerarAsync(comunidade, utilizadorId))
            throw new UnauthorizedAccessException(
                "Apenas o proprietario pode moderar comentarios."
            );

        var comentario = await _comentarioRepository.GetByIdAsync(comentarioId);
        if (comentario == null || comentario.ComunidadeId != comunidade.Id)
            throw new KeyNotFoundException("Comentario nao encontrado.");

        AplicarModeracao(comentario, dto.Acao, utilizadorId);
        comentario.Reportado = false;
        await _comentarioRepository.UpdateAsync(comentario);

        return ComentarioMapper.ToReadDTO(comentario);
    }

    private static void ValidarComentario(ComentarioCreateDTO dto)
    {
        var texto = dto.Texto?.Trim() ?? string.Empty;

        if (texto.Length == 0)
            throw new ArgumentException("Escreve um comentario.");

        if (texto.Length < 3)
            throw new ArgumentException("O comentario deve ter pelo menos 3 caracteres.");

        if (texto.Length > 600)
            throw new ArgumentException("O comentario nao pode exceder 600 caracteres.");
    }

    private Task NotificarComentarioAsync(Comentario comentario)
    {
        return Task.WhenAll(
            _comentarioObservers.Select(observer => observer.NotificarAsync(comentario))
        );
    }

    private async Task<bool> UtilizadorPodeModerarAsync(Comunidade comunidade, int utilizadorId)
    {
        if (comunidade.CreatedByUserId == utilizadorId)
            return true;

        return await _comunidadeRepository.IsProprietarioAsync(comunidade.Id, utilizadorId);
    }

    private static void AplicarModeracao(
        Comentario comentario,
        AcaoModeracaoComentario acao,
        int moderadorUtilizadorId
    )
    {
        switch (acao)
        {
            case AcaoModeracaoComentario.Ocultar:
                comentario.Visivel = false;
                comentario.EstadoModeracao = EstadoModeracaoComentario.Oculto;
                break;
            case AcaoModeracaoComentario.Remover:
                comentario.Visivel = false;
                comentario.EstadoModeracao = EstadoModeracaoComentario.Removido;
                break;
            case AcaoModeracaoComentario.Restaurar:
                comentario.Visivel = true;
                comentario.EstadoModeracao = EstadoModeracaoComentario.Visivel;
                break;
            default:
                throw new ArgumentException("Acao de moderacao invalida.");
        }

        comentario.ModeradoPorUtilizadorId = moderadorUtilizadorId;
        comentario.ModeradoEm = DateTime.UtcNow;
    }
}
