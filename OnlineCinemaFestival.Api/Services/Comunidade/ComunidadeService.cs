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

    public ComunidadeService(IComunidadeRepository comunidadeRepository, IUtilizadorRepository utilizadorRepository)
    {
        _comunidadeRepository = comunidadeRepository;
        _utilizadorRepository = utilizadorRepository;
    }

    public async Task<IEnumerable<ComunidadeReadDto>> GetAllComunidadesAsync(int usuarioIdPedido)
    {
        var comunidades = await _comunidadeRepository.FindComunidadesAsync(
            c => c.IsPublic && !c.Members.Any(m => m.UtilizadorId == usuarioIdPedido)
        );
        return comunidades.Select(ComunidadeMapper.ToReadDto);
    }

    public async Task<IEnumerable<ComunidadeReadDto>> GetMinhasComunidadesAsync(int usuarioId)
    {
        var comunidades = await _comunidadeRepository.FindComunidadesAsync(
            c => c.Members.Any(m => m.UtilizadorId == usuarioId)
        );

        return comunidades.Select(ComunidadeMapper.ToReadDto);
    }


    public async Task<ComunidadeReadDto?> GetComunidadeByPublicIdAsync(Guid publicId, int usuarioIdPedido)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(publicId);
        if (comunidade == null) return null;

       bool acessoProibido = !comunidade.IsPublic && !await _comunidadeRepository.IsMembroAsync(comunidade.Id, usuarioIdPedido);

        if (acessoProibido) throw new UnauthorizedAccessException("Acesso negado à comunidade privada");

        return ComunidadeMapper.ToReadDto(comunidade);
    }

    public async Task<ComunidadeReadDto> CreateComunidadeAsync(ComunidadeCreateDto dto, int criadorUserId)
    {
        var criadorUser = await _utilizadorRepository.GetByIdAsync(criadorUserId);
        if (criadorUser == null) throw new Exception("Criador não encontrado");

        
        var comunidadeEntity = ComunidadeMapper.ToEntity(dto, criadorUserId);

        comunidadeEntity.CodigoConvite = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

        comunidadeEntity.Members.Add(new ComunidadeMembro
        {
            UtilizadorId = criadorUserId,
            Role = CommunityMemberRole.Owner,
            JoinedAt = DateTime.UtcNow
        });

        var result = await _comunidadeRepository.AddComunidadeAsync(comunidadeEntity);

        // Carregar o usuário criador para incluir o nome no DTO de leitura
        result.CreatedByUser = criadorUser;

        return ComunidadeMapper.ToReadDto(result);

    }


    public async Task<ComunidadeReadDto?> GetComunidadeByConviteAsync(string codigoConvite)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByConviteAsync(codigoConvite);
        if (comunidade == null) return null;

        return ComunidadeMapper.ToReadDto(comunidade);
        
    }


}