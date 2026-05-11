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

    public async Task<IEnumerable<ComunidadeReadDto>> GetAllComunidadesAsync()
    {
        var comunidades = await _comunidadeRepository.GetAllComunidadesAsync();
        return comunidades.Select(ComunidadeMapper.ToReadDto);
    }

    public async Task<ComunidadeReadDto?> GetComunidadeByIdAsync(int id)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByIdAsync(id);
        if (comunidade == null) return null;
        return ComunidadeMapper.ToReadDto(comunidade);
    }

    public async Task<ComunidadeReadDto> CreateComunidadeAsync(ComunidadeCreateDto dto, int criadorUserId)
    {
        var criadorUser = await _utilizadorRepository.GetByIdAsync(criadorUserId);
        if (criadorUser == null) throw new Exception("Criador não encontrado");

        var comunidadeEntity = ComunidadeMapper.ToEntity(dto, criadorUserId);

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
}