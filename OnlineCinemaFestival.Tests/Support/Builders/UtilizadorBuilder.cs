using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class UtilizadorBuilder
{
    private int _id = 1;
    private string _name = "Utilizador Teste";
    private string _email = "utilizador@teste.pt";
    private string _phoneNumber = "+351910000000";
    private string _passwordHash = "hash";
    private PapelUtilizador _role = PapelUtilizador.Utilizador;
    private bool _isActive = true;
    private string _nationality = "PT";
    private DateTime _createdAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private PerfilUtilizador? _perfil;

    public UtilizadorBuilder ComId(int id) { _id = id; return this; }
    public UtilizadorBuilder ComNome(string nome) { _name = nome; return this; }
    public UtilizadorBuilder ComEmail(string email) { _email = email; return this; }
    public UtilizadorBuilder ComTelefone(string telefone) { _phoneNumber = telefone; return this; }
    public UtilizadorBuilder ComPasswordHash(string hash) { _passwordHash = hash; return this; }
    public UtilizadorBuilder ComoAdministrador() { _role = PapelUtilizador.Administrador; return this; }
    public UtilizadorBuilder Inativo() { _isActive = false; return this; }
    public UtilizadorBuilder ComNacionalidade(string nac) { _nationality = nac; return this; }
    public UtilizadorBuilder CriadoEm(DateTime data) { _createdAt = data; return this; }
    public UtilizadorBuilder ComPerfil(PerfilUtilizador perfil) { _perfil = perfil; return this; }

    public Utilizador Build()
    {
        var utilizador = new Utilizador
        {
            Id = _id,
            Name = _name,
            Email = _email,
            PhoneNumber = _phoneNumber,
            PasswordHash = _passwordHash,
            Role = _role,
            IsActive = _isActive,
            Nationality = _nationality,
            CreatedAt = _createdAt,
        };

        if (_perfil != null)
        {
            _perfil.UtilizadorId = _id;
            utilizador.Perfil = _perfil;
        }

        return utilizador;
    }

    public static implicit operator Utilizador(UtilizadorBuilder builder) => builder.Build();
}
