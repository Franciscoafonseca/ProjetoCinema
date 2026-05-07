using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FestivalFilmeService : IFestivalFilmeService
{
    private readonly IFestivalFilmeRepository _festivalFilmeRepository;
    private readonly IFestivalRepository _festivalRepository;
    private readonly IFilmeRepository _filmeRepository;

    public FestivalFilmeService(
        IFestivalFilmeRepository festivalFilmeRepository,
        IFestivalRepository festivalRepository,
        IFilmeRepository filmeRepository
    )
    {
        _festivalFilmeRepository = festivalFilmeRepository;
        _festivalRepository = festivalRepository;
        _filmeRepository = filmeRepository;
    }

    public async Task AssociarFilmeAsync(int festivalId, int filmeId)
    {
        var festival = await _festivalRepository.GetByIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        var filme = await _filmeRepository.GetByIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme não encontrado.");

        var alreadyExists = await _festivalFilmeRepository.ExistsAsync(festivalId, filmeId);

        if (alreadyExists)
            throw new InvalidOperationException("Este filme já está associado a este festival.");

        var festivalFilme = new FestivalFilme { FestivalId = festivalId, FilmeId = filmeId };

        await _festivalFilmeRepository.AddAsync(festivalFilme);
        await _festivalFilmeRepository.SaveChangesAsync();
    }

    public async Task RemoverFilmeAsync(int festivalId, int filmeId)
    {
        var festivalFilme = await _festivalFilmeRepository.GetAsync(festivalId, filmeId);

        if (festivalFilme == null)
            throw new KeyNotFoundException("Associação entre festival e filme não encontrada.");

        _festivalFilmeRepository.Remove(festivalFilme);
        await _festivalFilmeRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<FilmeReadDto>> GetFilmesByFestivalAsync(int festivalId)
    {
        var festival = await _festivalRepository.GetByIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        var filmes = await _festivalFilmeRepository.GetFilmesByFestivalIdAsync(festivalId);

        return filmes.Select(FilmeMapper.MapToReadDto);
    }
}
