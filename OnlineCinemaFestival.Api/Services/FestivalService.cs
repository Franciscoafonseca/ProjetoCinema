using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FestivalService : IFestivalService
{
    private readonly IFestivalRepository _repository;

    public FestivalService(IFestivalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FestivalReadDto>> GetAllAsync()
    {
        var festivals = await _repository.GetAllAsync();

        return festivals.Select(FestivalMapper.MapToReadDto);
    }

    public async Task<FestivalReadDto?> GetByIdAsync(int id)
    {
        var festival = await _repository.GetByIdAsync(id);

        if (festival == null)
            return null;

        return FestivalMapper.MapToReadDto(festival);
    }

    public async Task<FestivalReadDto> CreateAsync(FestivalCreateDto dto)
    {
        ValidateFestivalData(dto.Name, dto.StartDate, dto.EndDate);

        var festival = FestivalMapper.MapFromCreateDto(dto);

        await _repository.AddAsync(festival);
        await _repository.SaveChangesAsync();

        return FestivalMapper.MapToReadDto(festival);
    }

    public async Task UpdateAsync(int id, FestivalUpdateDto dto)
    {
        ValidateFestivalData(dto.Name, dto.StartDate, dto.EndDate);

        var festival = await _repository.GetByIdAsync(id);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        FestivalMapper.MapToExistingFestival(dto, festival);

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var festival = await _repository.GetByIdAsync(id);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        _repository.Remove(festival);

        await _repository.SaveChangesAsync();
    }

    private static void ValidateFestivalData(string name, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do festival é obrigatório.");

        if (endDate < startDate)
            throw new ArgumentException("A data de fim não pode ser anterior à data de início.");
    }
}
