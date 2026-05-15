using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class GeneroService : IGeneroService
{
    private readonly IGeneroRepository _generoRepository;

    public GeneroService(IGeneroRepository generoRepository)
    {
        _generoRepository = generoRepository;
    }

    public async Task<IEnumerable<GeneroDto>> GetAllAsync()
    {
        var generos = await _generoRepository.GetAllAsync();

        return generos.Select(GeneroMapper.MapToDto);
    }

    public async Task<GeneroDto> CreateAsync(CriarGeneroDto dto)
    {
        var genero = GeneroMapper.MapFromCreateDto(dto);

        genero.Name = genero.Name.Trim();

        if (string.IsNullOrWhiteSpace(genero.Name))
            throw new ArgumentException("O nome do género é obrigatório.");

        await _generoRepository.AddAsync(genero);
        await _generoRepository.SaveChangesAsync();

        return GeneroMapper.MapToDto(genero);
    }
}
