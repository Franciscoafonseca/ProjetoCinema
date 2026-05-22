using System.Text.RegularExpressions;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class PerfilUtilizadorService : IPerfilUtilizadorService
{
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IGeneroRepository _generoRepository;
    private readonly IPerfilFotoUploadService _fotoUploadService;

    public PerfilUtilizadorService(
        IUtilizadorRepository utilizadorRepository,
        IGeneroRepository generoRepository,
        IPerfilFotoUploadService fotoUploadService
    )
    {
        _utilizadorRepository = utilizadorRepository;
        _generoRepository = generoRepository;
        _fotoUploadService = fotoUploadService;
    }

    public async Task<PerfilPrivadoDTO> ObterMeuPerfilAsync(int userId)
    {
        var utilizador = await _utilizadorRepository.ObterComPerfilAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new ArgumentException("Perfil nao encontrado.");

        return ToPrivadoDTO(utilizador);
    }

    public async Task<PerfilPrivadoDTO> AtualizarMeuPerfilAsync(
        int userId,
        PedidoAtualizarPerfilDTO request
    )
    {
        var utilizador = await _utilizadorRepository.ObterComPerfilAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new ArgumentException("Perfil nao encontrado.");

        if (!string.IsNullOrWhiteSpace(request.Name))
            utilizador.Name = request.Name.Trim();

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var telefone = NormalizarTelefone(request.PhoneNumber);
            var existente = await _utilizadorRepository.ObterPorTelefoneAsync(telefone);

            if (existente != null && existente.Id != utilizador.Id)
                throw new ArgumentException("Ja existe um utilizador com este telefone.");

            utilizador.PhoneNumber = telefone;
        }

        var pais = PerfilOpcoes.ObterPaisValido(
            string.IsNullOrWhiteSpace(request.CountryCode) ? request.Nationality : request.CountryCode
        );
        var localidade = PerfilOpcoes.ObterLocalidadeValida(request.Location);

        utilizador.Nationality = pais.Codigo;
        utilizador.Perfil.Nationality = pais.Nome;
        utilizador.Perfil.CountryCode = pais.Codigo;
        utilizador.Perfil.Bio = request.Bio.Trim();
        utilizador.Perfil.Location = localidade;
        utilizador.Perfil.IsPublic = request.IsPublic;

        utilizador.Perfil.UpdatedAt = DateTime.UtcNow;
        utilizador.UpdatedAt = DateTime.UtcNow;

        var generos = await _generoRepository.ObterPorIdsAsync(request.FavoriteGenreIds);

        if (generos.Count != request.FavoriteGenreIds.Distinct().Count())
            throw new ArgumentException("Um ou mais generos favoritos nao existem.");

        utilizador.GenerosFavoritos.Clear();

        foreach (var genero in generos)
        {
            utilizador.GenerosFavoritos.Add(
                new UtilizadorGeneroFavorito { UtilizadorId = utilizador.Id, GeneroId = genero.Id }
            );
        }

        await _utilizadorRepository.SaveChangesAsync();

        return ToPrivadoDTO(utilizador);
    }

    public async Task<PerfilPrivadoDTO> EnviarFotoPerfilAsync(int userId, IFormFile ficheiro)
    {
        var utilizador = await _utilizadorRepository.ObterComPerfilAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new ArgumentException("Perfil nao encontrado.");

        utilizador.Perfil.ProfileImageUrl = await _fotoUploadService.GuardarAsync(ficheiro);
        utilizador.Perfil.UpdatedAt = DateTime.UtcNow;
        utilizador.UpdatedAt = DateTime.UtcNow;

        await _utilizadorRepository.SaveChangesAsync();

        return ToPrivadoDTO(utilizador);
    }

    public async Task<List<PerfilPublicoDTO>> ObterPerfisPublicosAsync()
    {
        var utilizadores = await _utilizadorRepository.ObterPerfisPublicosAsync();
        return utilizadores.Select(ToPublicoDTO).ToList();
    }

    public async Task<PerfilPublicoDTO> ObterPerfilPublicoAsync(int userId)
    {
        var utilizador = await _utilizadorRepository.ObterComPerfilAsync(userId);

        if (utilizador == null || utilizador.Perfil == null || !utilizador.Perfil.IsPublic)
            throw new ArgumentException("Perfil publico nao encontrado.");

        return ToPublicoDTO(utilizador);
    }

    private static PerfilPublicoDTO ToPublicoDTO(Utilizador utilizador)
    {
        var countryCode = ObterCountryCodePerfil(utilizador);

        return new PerfilPublicoDTO
        {
            UserId = utilizador.Id,
            Name = utilizador.Name,
            Nationality = ObterNacionalidadePerfil(utilizador),
            CountryCode = countryCode,
            CountryFlag = PerfilOpcoes.ObterBandeira(countryCode),
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

    private static PerfilPrivadoDTO ToPrivadoDTO(Utilizador utilizador)
    {
        var publico = ToPublicoDTO(utilizador);

        return new PerfilPrivadoDTO
        {
            UserId = publico.UserId,
            Name = publico.Name,
            Email = utilizador.Email,
            PhoneNumber = utilizador.PhoneNumber,
            Nationality = publico.Nationality,
            CountryCode = publico.CountryCode,
            CountryFlag = publico.CountryFlag,
            Bio = publico.Bio,
            ProfileImageUrl = publico.ProfileImageUrl,
            Location = publico.Location,
            IsPublic = publico.IsPublic,
            FavoriteGenres = publico.FavoriteGenres,
            ReviewsCount = publico.ReviewsCount,
            CommunitiesCount = publico.CommunitiesCount,
            PublicListsCount = publico.PublicListsCount,
        };
    }

    private static string ObterNacionalidadePerfil(Utilizador utilizador)
    {
        if (!string.IsNullOrWhiteSpace(utilizador.Perfil?.Nationality))
            return utilizador.Perfil.Nationality;

        if (string.IsNullOrWhiteSpace(utilizador.Nationality))
            return string.Empty;

        return PerfilOpcoes.Paises.FirstOrDefault(p =>
                p.Codigo.Equals(utilizador.Nationality, StringComparison.OrdinalIgnoreCase)
                || p.Nome.Equals(utilizador.Nationality, StringComparison.OrdinalIgnoreCase)
            )
            ?.Nome ?? utilizador.Nationality;
    }

    private static string ObterCountryCodePerfil(Utilizador utilizador)
    {
        if (!string.IsNullOrWhiteSpace(utilizador.Perfil?.CountryCode))
            return utilizador.Perfil.CountryCode.ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(utilizador.Nationality))
            return string.Empty;

        var pais = PerfilOpcoes.Paises.FirstOrDefault(p =>
            p.Codigo.Equals(utilizador.Nationality, StringComparison.OrdinalIgnoreCase)
            || p.Nome.Equals(utilizador.Nationality, StringComparison.OrdinalIgnoreCase)
        );

        return pais?.Codigo ?? string.Empty;
    }

    private static string NormalizarTelefone(string telefone)
    {
        var normalizado = Regex.Replace(telefone.Trim(), @"[\s().-]", "");

        if (!Regex.IsMatch(normalizado, @"^\+?[0-9]{7,15}$"))
            throw new ArgumentException("Introduz um telefone valido.");

        return normalizado;
    }
}
