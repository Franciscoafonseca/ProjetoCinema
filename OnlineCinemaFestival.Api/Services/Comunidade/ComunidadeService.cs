using Microsoft.AspNetCore.Identity;
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

    public async Task<IEnumerable<ComunidadeReadDTO>> ObterTodasComunidadesAsync(int usuarioIdPedido)
    {
        var comunidades = await _comunidadeRepository.FindComunidadesAsync(c =>
            c.IsPublic && !c.Members.Any(m => m.UtilizadorId == usuarioIdPedido)
        );
        return comunidades.Select(ComunidadeMapper.ToReadDTO);
    }

    public async Task<IEnumerable<ComunidadeReadDTO>> ObterMinhasComunidadesAsync(int usuarioId)
    {
        var comunidades = await _comunidadeRepository.FindComunidadesAsync(c =>
            c.Members.Any(m => m.UtilizadorId == usuarioId)
        );

        return comunidades.Select(ComunidadeMapper.ToReadDTO);
    }

    public async Task<ComunidadeReadDTO?> ObterComunidadePorIdAsync(int id, int usuarioIdPedido)
    {
        var comunidade = await _comunidadeRepository.ObterComunidadePorIdAsync(id);
        if (comunidade == null)
            return null;

        bool acessoProibido =
            !comunidade.IsPublic && !await _comunidadeRepository.IsMembroAsync(id, usuarioIdPedido);

        if (acessoProibido)
            throw new UnauthorizedAccessException("Acesso negado à comunidade privada");

        return ComunidadeMapper.ToReadDTO(comunidade);
    }

    public async Task<ComunidadeReadDTO> CreateComunidadeAsync(
        ComunidadeCreateDTO dto,
        int criadorUserId
    )
    {
        var criadorUser = await _utilizadorRepository.ObterPorIdAsync(criadorUserId);
        if (criadorUser == null)
            throw new Exception("Criador não encontrado");

        var comunidadeEntity = ComunidadeMapper.ToEntity(dto, criadorUserId);

        comunidadeEntity.Members.Add(
            new ComunidadeMembro
            {
                UtilizadorId = criadorUserId,
                Role = PapelMembroComunidade.Proprietario,
                JoinedAt = DateTime.UtcNow,
            }
        );

        var result = await _comunidadeRepository.AddComunidadeAsync(comunidadeEntity);

        // Carregar o usuário criador para incluir o nome no DTO de leitura
        result.CreatedByUser = criadorUser;

        return ComunidadeMapper.ToReadDTO(result);
    }
}
