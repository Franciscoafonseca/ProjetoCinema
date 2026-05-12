using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class GeneroService : IGeneroService
{
    private readonly IGeneroRepository _generoRepository;

    public GeneroService(IGeneroRepository generoRepository)
    {
        _generoRepository = generoRepository;
    }

    public async Task<IEnumerable<Genero>> GetAllAsync()
    {
        return await _generoRepository.GetAllAsync();
    }

    public async Task<Genero> CreateAsync(Genero genero)
    {
        genero.Name = genero.Name.Trim();

        if (string.IsNullOrWhiteSpace(genero.Name))
            throw new ArgumentException("O nome do género é obrigatório.");

        await _generoRepository.AddAsync(genero);
        await _generoRepository.SaveChangesAsync();

        return genero;
    }
}
