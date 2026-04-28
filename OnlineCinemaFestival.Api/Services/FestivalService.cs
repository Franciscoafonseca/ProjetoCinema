using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FestivalService
{
    private readonly FestivalRepository _repository;

    public FestivalService(FestivalRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Festival>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Festival?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddAsync(Festival festival)
    {
        if (string.IsNullOrWhiteSpace(festival.Name))
            throw new ArgumentException("O nome do festival é obrigatório.");

        if (festival.EndDate < festival.StartDate)
            throw new ArgumentException("A data de fim não pode ser anterior à data de início.");

        await _repository.AddAsync(festival);
    }
}
