using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Serviço responsável pela gestão dos perfis de utilizador.
/// Permite consultar e atualizar o próprio perfil, bem como consultar perfis públicos.
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IGeneroRepository _generoRepository;

    /// <summary>
    /// Inicializa uma nova instância do serviço de perfis de utilizador.
    /// </summary>
    /// <param name="utilizadorRepository">
    /// Repositório responsável pelo acesso aos dados dos utilizadores.
    /// </param>
    /// <param name="generoRepository">
    /// Repositório responsável pelo acesso aos géneros cinematográficos.
    /// </param>
    public UserProfileService(
        IUtilizadorRepository utilizadorRepository,
        IGeneroRepository generoRepository
    )
    {
        _utilizadorRepository = utilizadorRepository;
        _generoRepository = generoRepository;
    }

    /// <summary>
    /// Obtém o perfil privado do utilizador autenticado.
    /// </summary>
    /// <param name="userId">Identificador do utilizador autenticado.</param>
    /// <returns>Dados completos do perfil do utilizador, incluindo email.</returns>
    /// <exception cref="ArgumentException">
    /// Lançada quando o utilizador ou o perfil não são encontrados.
    /// </exception>
    public async Task<UserProfileResponse> GetMyProfileAsync(int userId)
    {
        // Obtém o utilizador juntamente com o respetivo perfil e dados relacionados.
        var utilizador = await _utilizadorRepository.GetWithProfileAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new ArgumentException("Perfil não encontrado.");

        // Devolve o perfil completo, incluindo o email por se tratar do próprio utilizador.
        return ToResponse(utilizador, includeEmail: true);
    }

    /// <summary>
    /// Atualiza o perfil do utilizador autenticado.
    /// </summary>
    /// <param name="userId">Identificador do utilizador autenticado.</param>
    /// <param name="request">Dados atualizados do perfil.</param>
    /// <returns>Perfil atualizado do utilizador.</returns>
    /// <exception cref="ArgumentException">
    /// Lançada quando o utilizador ou o perfil não são encontrados.
    /// </exception>
    public async Task<UserProfileResponse> UpdateMyProfileAsync(
        int userId,
        UpdateProfileRequest request
    )
    {
        // Obtém o utilizador juntamente com o seu perfil.
        var utilizador = await _utilizadorRepository.GetWithProfileAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new ArgumentException("Perfil não encontrado.");

        // Atualiza o nome apenas se tiver sido enviado um valor válido.
        if (!string.IsNullOrWhiteSpace(request.Name))
            utilizador.Name = request.Name.Trim();

        // Atualiza os dados pessoais e públicos do perfil.
        utilizador.Nationality = request.Nationality.Trim();
        utilizador.Perfil.Bio = request.Bio.Trim();
        utilizador.Perfil.ProfileImageUrl = request.ProfileImageUrl.Trim();
        utilizador.Perfil.Location = request.Location.Trim();
        utilizador.Perfil.IsPublic = request.IsPublic;

        // Regista as datas de atualização do perfil e do utilizador.
        utilizador.Perfil.UpdatedAt = DateTime.UtcNow;
        utilizador.UpdatedAt = DateTime.UtcNow;

        // Obtém os géneros favoritos selecionados pelo utilizador.
        var generos = await _generoRepository.GetByIdsAsync(request.FavoriteGenreIds);

        if (generos.Count != request.FavoriteGenreIds.Distinct().Count())
        {
            throw new ArgumentException("Um ou mais géneros favoritos não existem.");
        }

        utilizador.GenerosFavoritos.Clear();

        foreach (var genero in generos)
        {
            utilizador.GenerosFavoritos.Add(
                new UtilizadorGeneroFavorito { UtilizadorId = utilizador.Id, GeneroId = genero.Id }
            );
        }

        // Guarda todas as alterações efetuadas ao perfil.
        await _utilizadorRepository.SaveChangesAsync();

        // Devolve o perfil atualizado, incluindo o email do próprio utilizador.
        return ToResponse(utilizador, includeEmail: true);
    }

    /// <summary>
    /// Obtém todos os perfis públicos existentes no sistema.
    /// </summary>
    /// <returns>Lista de perfis públicos, sem exposição dos emails dos utilizadores.</returns>
    public async Task<List<UserProfileResponse>> GetPublicProfilesAsync()
    {
        // Obtém apenas utilizadores com perfil público.
        var utilizadores = await _utilizadorRepository.GetPublicProfilesAsync();

        // Converte os utilizadores para resposta pública, ocultando o email.
        return utilizadores.Select(u => ToResponse(u, includeEmail: false)).ToList();
    }

    /// <summary>
    /// Obtém o perfil público de um utilizador específico.
    /// </summary>
    /// <param name="userId">Identificador do utilizador.</param>
    /// <returns>Dados públicos do perfil do utilizador.</returns>
    /// <exception cref="ArgumentException">
    /// Lançada quando o utilizador não existe, não tem perfil ou o perfil não é público.
    /// </exception>
    public async Task<UserProfileResponse> GetPublicProfileAsync(int userId)
    {
        // Obtém o utilizador com o respetivo perfil.
        var utilizador = await _utilizadorRepository.GetWithProfileAsync(userId);

        // Garante que o perfil existe e está definido como público.
        if (utilizador == null || utilizador.Perfil == null || !utilizador.Perfil.IsPublic)
            throw new ArgumentException("Perfil público não encontrado.");

        // Devolve apenas informação pública, sem expor o email.
        return ToResponse(utilizador, includeEmail: false);
    }

    /// <summary>
    /// Converte uma entidade Utilizador para um DTO de resposta de perfil.
    /// </summary>
    /// <param name="utilizador">Utilizador a converter.</param>
    /// <param name="includeEmail">
    /// Indica se o email deve ser incluído na resposta.
    /// Deve ser verdadeiro apenas quando o perfil pertence ao próprio utilizador autenticado.
    /// </param>
    /// <returns>DTO com os dados do perfil do utilizador.</returns>
    private static UserProfileResponse ToResponse(Utilizador utilizador, bool includeEmail)
    {
        // Mapeia a entidade Utilizador para um DTO adequado à resposta da API.
        return new UserProfileResponse
        {
            UserId = utilizador.Id,
            Name = utilizador.Name,

            // O email só é apresentado quando o utilizador consulta o seu próprio perfil.
            Email = includeEmail ? utilizador.Email : string.Empty,

            Nationality = utilizador.Nationality,
            Bio = utilizador.Perfil?.Bio ?? string.Empty,
            ProfileImageUrl = utilizador.Perfil?.ProfileImageUrl ?? string.Empty,
            Location = utilizador.Perfil?.Location ?? string.Empty,
            IsPublic = utilizador.Perfil?.IsPublic ?? false,

            // Lista os géneros favoritos por ordem alfabética.
            FavoriteGenres = utilizador
                .GenerosFavoritos.Select(g => g.Genero.Name)
                .OrderBy(name => name)
                .ToList(),

            // Calcula estatísticas simples associadas ao perfil.
            ReviewsCount = utilizador.Avaliacoes.Count,
            CommunitiesCount = utilizador.Comunidades.Count,
            PublicListsCount = utilizador.ListasPessoais.Count(l => l.IsPublic),
        };
    }
}
