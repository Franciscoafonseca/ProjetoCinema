using System.Text.RegularExpressions;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class PerfilUtilizadorService : IPerfilUtilizadorService
{
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IGeneroRepository _generoRepository;
    private readonly IPerfilFotoUploadService _fotoUploadService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PerfilUtilizadorService(
        IUtilizadorRepository utilizadorRepository,
        IGeneroRepository generoRepository,
        IPerfilFotoUploadService fotoUploadService,
        IHttpContextAccessor? httpContextAccessor = null
    )
    {
        _utilizadorRepository = utilizadorRepository;
        _generoRepository = generoRepository;
        _fotoUploadService = fotoUploadService;
        _httpContextAccessor = httpContextAccessor ?? new HttpContextAccessor();
    }

    public async Task<PerfilPrivadoDTO> ObterMeuPerfilAsync(int userId)
    {
        var utilizador = await _utilizadorRepository.ObterComPerfilAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new KeyNotFoundException("Perfil nao encontrado.");

        return ToPrivadoDTO(utilizador);
    }

    public async Task<PerfilPrivadoDTO> AtualizarMeuPerfilAsync(
        int userId,
        PedidoAtualizarPerfilDTO request
    )
    {
        var utilizador = await _utilizadorRepository.ObterComPerfilAsync(userId);

        if (utilizador == null || utilizador.Perfil == null)
            throw new KeyNotFoundException("Perfil nao encontrado.");

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
            throw new KeyNotFoundException("Perfil nao encontrado.");

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
            throw new KeyNotFoundException("Perfil publico nao encontrado.");

        return ToPublicoDTO(utilizador);
    }

    private PerfilPublicoDTO ToPublicoDTO(Utilizador utilizador)
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
            ProfileImageUrl = ResolverUrlPublica(utilizador.Perfil?.ProfileImageUrl),
            Location = utilizador.Perfil?.Location ?? string.Empty,
            IsPublic = utilizador.Perfil?.IsPublic ?? false,
            FavoriteGenres = utilizador
                .GenerosFavoritos.Select(g => g.Genero.Name)
                .OrderBy(name => name)
                .ToList(),
            ReviewsCount = utilizador.Avaliacoes.Count,
            CommunitiesCount = utilizador.Comunidades.Count,
            PublicListsCount = utilizador.ListasPessoais.Count(l => l.IsPublic),
            PublicLists = utilizador
                .ListasPessoais.Where(l => l.IsPublic)
                .OrderBy(l => l.Name)
                .Select(l => new ListaPessoalPublicaDTO
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description,
                    TotalFilmes = l.Items?.Count ?? 0,
                })
                .ToList(),
            PublicReviews = utilizador
                .Avaliacoes.OrderByDescending(a => a.Data)
                .Select(a => new ReviewPublicaDTO
                {
                    Id = a.Id,
                    FilmeId = a.FilmeId,
                    FilmeTitulo = a.Filme?.Titulo ?? string.Empty,
                    Pontuacao = a.Pontuacao,
                    Texto = a.Texto,
                    Data = a.Data,
                })
                .ToList(),
            PublicCommunities = utilizador
                .Comunidades.Where(m => m.Comunidade.IsPublic)
                .OrderBy(m => m.Comunidade.Name)
                .Select(m => new ComunidadePublicaPerfilDTO
                {
                    Id = m.Comunidade.PublicId,
                    Name = m.Comunidade.Name,
                    Description = m.Comunidade.Description,
                    ImageUrl = ResolverUrlPublica(m.Comunidade.ImageUrl),
                    MembersCount = m.Comunidade.Members?.Count ?? 0,
                })
                .ToList(),
        };
    }

    private PerfilPrivadoDTO ToPrivadoDTO(Utilizador utilizador)
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
            PublicCommunities = publico.PublicCommunities,
        };
    }

    private string ResolverUrlPublica(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        var valor = url.Trim();

        if (Uri.TryCreate(valor, UriKind.Absolute, out _))
            return valor;

        if (valor.StartsWith("//", StringComparison.Ordinal))
        {
            var scheme = _httpContextAccessor.HttpContext?.Request.Scheme ?? "http";
            return $"{scheme}:{valor}";
        }

        var request = _httpContextAccessor.HttpContext?.Request;

        if (request == null)
            return valor;

        return $"{request.Scheme}://{request.Host}{request.PathBase}/{valor.TrimStart('/')}";
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
