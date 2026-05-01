using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IGeneroRepository _generoRepository;

    public UserProfileService(
        IUtilizadorRepository utilizadorRepository,
        IGeneroRepository generoRepository
    )
    {
        _utilizadorRepository = utilizadorRepository;
        _generoRepository = generoRepository;
    }

    public async Task<UserProfileResponse> GetMyProfileAsync(int userId)
    {
        var utilizador = await _utilizadorRepository.GetWithProfileAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new ArgumentException("Perfil não encontrado.");

        return ToResponse(utilizador, includeEmail: true);
    }

    public async Task<UserProfileResponse> UpdateMyProfileAsync(
        int userId,
        UpdateProfileRequest request
    )
    {
        var utilizador = await _utilizadorRepository.GetWithProfileAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new ArgumentException("Perfil não encontrado.");

        if (!string.IsNullOrWhiteSpace(request.Name))
            utilizador.Name = request.Name.Trim();

        utilizador.Nationality = request.Nationality.Trim();
        utilizador.Perfil.Bio = request.Bio.Trim();
        utilizador.Perfil.ProfileImageUrl = request.ProfileImageUrl.Trim();
        utilizador.Perfil.Location = request.Location.Trim();
        utilizador.Perfil.IsPublic = request.IsPublic;
        utilizador.Perfil.UpdatedAt = DateTime.UtcNow;
        utilizador.UpdatedAt = DateTime.UtcNow;

        var generos = await _generoRepository.GetByIdsAsync(request.FavoriteGenreIds);

        utilizador.GenerosFavoritos.Clear();

        foreach (var genero in generos)
        {
            utilizador.GenerosFavoritos.Add(
                new UtilizadorGeneroFavorito
                {
                    UtilizadorId = utilizador.Id,
                    GeneroId = genero.Id,
                    CreatedAt = DateTime.UtcNow,
                }
            );
        }

        await _utilizadorRepository.SaveChangesAsync();

        return ToResponse(utilizador, includeEmail: true);
    }

    public async Task<List<UserProfileResponse>> GetPublicProfilesAsync()
    {
        var utilizadores = await _utilizadorRepository.GetPublicProfilesAsync();

        return utilizadores.Select(u => ToResponse(u, includeEmail: false)).ToList();
    }

    public async Task<UserProfileResponse> GetPublicProfileAsync(int userId)
    {
        var utilizador = await _utilizadorRepository.GetWithProfileAsync(userId);

        if (utilizador == null || utilizador.Perfil == null || !utilizador.Perfil.IsPublic)
            throw new ArgumentException("Perfil público não encontrado.");

        return ToResponse(utilizador, includeEmail: false);
    }

    private static UserProfileResponse ToResponse(Utilizador utilizador, bool includeEmail)
    {
        return new UserProfileResponse
        {
            UserId = utilizador.Id,
            Name = utilizador.Name,
            Email = includeEmail ? utilizador.Email : string.Empty,
            Nationality = utilizador.Nationality,
            Bio = utilizador.Perfil?.Bio ?? string.Empty,
            ProfileImageUrl = utilizador.Perfil?.ProfileImageUrl ?? string.Empty,
            Location = utilizador.Perfil?.Location ?? string.Empty,
            IsPublic = utilizador.Perfil?.IsPublic ?? false,
            FavoriteGenres = utilizador
                .GenerosFavoritos.Select(g => g.Genero.Name)
                .OrderBy(name => name)
                .ToList(),
            ReviewsCount = utilizador.Avaliacoes.Count,
            CommunitiesCount = utilizador.Comunidades.Count,
            PublicListsCount = utilizador.ListasPessoais.Count(l => l.IsPublic),
        };
    }
}
