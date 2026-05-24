using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class PerfilBuilder
{
    private int _utilizadorId;
    private string _bio = "Bio de teste";
    private string _profileImageUrl = string.Empty;
    private string _nationality = "Portugal";
    private string _countryCode = "PT";
    private string _location = "Lisboa";
    private bool _isPublic = true;

    public PerfilBuilder DoUtilizador(int utilizadorId) { _utilizadorId = utilizadorId; return this; }
    public PerfilBuilder ComBio(string bio) { _bio = bio; return this; }
    public PerfilBuilder ComImagem(string url) { _profileImageUrl = url; return this; }
    public PerfilBuilder ComNacionalidade(string nac) { _nationality = nac; return this; }
    public PerfilBuilder ComCodigoPais(string codigo) { _countryCode = codigo; return this; }
    public PerfilBuilder Em(string localizacao) { _location = localizacao; return this; }
    public PerfilBuilder Privado() { _isPublic = false; return this; }
    public PerfilBuilder Publico() { _isPublic = true; return this; }

    public PerfilUtilizador Build() => new()
    {
        UtilizadorId = _utilizadorId,
        Bio = _bio,
        ProfileImageUrl = _profileImageUrl,
        Nationality = _nationality,
        CountryCode = _countryCode,
        Location = _location,
        IsPublic = _isPublic,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
    };

    public static implicit operator PerfilUtilizador(PerfilBuilder builder) => builder.Build();
}
