using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ComunidadeService : IComunidadeService
{
    private readonly IComunidadeRepository _comunidadeRepository;
    private readonly IUtilizadorRepository _utilizadorRepository;

    public ComunidadeService(
        IComunidadeRepository comunidadeRepository,
        IUtilizadorRepository utilizadorRepository
    )
    {
        _comunidadeRepository = comunidadeRepository;
        _utilizadorRepository = utilizadorRepository;
    }

    public async Task<IEnumerable<ComunidadeReadDTO>> ObterTodasComunidadesAsync(
        int utilizadorIdPedido
    )
    {
        var comunidades = await _comunidadeRepository.FindComunidadesAsync(c =>
            c.IsPublic && !c.Members.Any(m => m.UtilizadorId == utilizadorIdPedido)
        );

        return comunidades.Select(ComunidadeMapper.ToReadDTO);
    }

    public async Task<IEnumerable<ComunidadeReadDTO>> ObterMinhasComunidadesAsync(int utilizadorId)
    {
        var comunidades = await _comunidadeRepository.FindComunidadesAsync(c =>
            c.Members.Any(m => m.UtilizadorId == utilizadorId)
        );

        return comunidades.Select(ComunidadeMapper.ToReadDTO);
    }

    public async Task<ComunidadeReadDTO?> ObterComunidadePorPublicIdAsync(
        Guid publicId,
        int utilizadorIdPedido
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(publicId);

        if (comunidade == null)
            return null;

        var acessoProibido =
            !comunidade.IsPublic
            && !await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizadorIdPedido);

        if (acessoProibido)
            throw new UnauthorizedAccessException("Acesso negado à comunidade privada.");

        return ComunidadeMapper.ToReadDTO(comunidade);
    }

    public async Task<ComunidadeReadDTO> CriarComunidadeAsync(
        ComunidadeCreateDTO dto,
        int criadorUtilizadorId
    )
    {
        var criador = await _utilizadorRepository.ObterPorIdAsync(criadorUtilizadorId);

        if (criador == null)
            throw new Exception("Criador não encontrado.");

        var comunidade = ComunidadeMapper.ToEntity(dto, criadorUtilizadorId);

        comunidade.CodigoConvite = Guid.NewGuid().ToString("N")[..8].ToUpper();

        comunidade.Members.Add(
            new ComunidadeMembro
            {
                UtilizadorId = criadorUtilizadorId,
                Role = PapelMembroComunidade.Proprietario,
                JoinedAt = DateTime.UtcNow,
            }
        );

        var comunidadeCriada = await _comunidadeRepository.AddComunidadeAsync(comunidade);

        comunidadeCriada.CreatedByUser = criador;

        return ComunidadeMapper.ToReadDTO(comunidadeCriada);
    }

    public async Task<ComunidadeReadDTO?> ObterComunidadePorConviteAsync(string codigoConvite)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByConviteAsync(codigoConvite);

        if (comunidade == null)
            return null;

        return ComunidadeMapper.ToReadDTO(comunidade);
    }

    public async Task AderirComunidadeAsync(Guid comunidadePublicId, int utilizadorId)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(
            comunidadePublicId
        );

        var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);

        await ValidarRegrasDeAdesaoAsync(comunidade, utilizador, entradaPorConvite: false);

        var novoMembro = new ComunidadeMembro
        {
            ComunidadeId = comunidade!.Id,
            UtilizadorId = utilizadorId,
            Role = PapelMembroComunidade.Membro,
            JoinedAt = DateTime.UtcNow,
        };

        await _comunidadeRepository.AdicionarMembroAsync(novoMembro);
    }

    public async Task AderirComunidadePorConviteAsync(string codigoConvite, int utilizadorId)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByConviteAsync(codigoConvite);

        var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);

        await ValidarRegrasDeAdesaoAsync(comunidade, utilizador, entradaPorConvite: true);

        var novoMembro = new ComunidadeMembro
        {
            ComunidadeId = comunidade!.Id,
            UtilizadorId = utilizadorId,
            Role = PapelMembroComunidade.Membro,
            JoinedAt = DateTime.UtcNow,
        };

        await _comunidadeRepository.AdicionarMembroAsync(novoMembro);
    }

    private async Task ValidarRegrasDeAdesaoAsync(
        Comunidade? comunidade,
        Utilizador? utilizador,
        bool entradaPorConvite
    )
    {
        if (utilizador == null)
            throw new Exception("Utilizador não encontrado.");

        if (comunidade == null)
            throw new Exception("Comunidade não encontrada.");

        var jaEMembro = await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizador.Id);

        if (jaEMembro)
            throw new Exception("Já és membro desta comunidade.");

        if (!entradaPorConvite && !comunidade.IsPublic)
            throw new UnauthorizedAccessException(
                "Esta comunidade é privada. Precisas de um convite."
            );
    }
}
