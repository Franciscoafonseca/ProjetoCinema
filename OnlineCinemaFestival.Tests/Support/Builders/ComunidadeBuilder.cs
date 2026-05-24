using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class ComunidadeBuilder
{
    private int _id = 1;
    private Guid _publicId = Guid.NewGuid();
    private string _name = "Comunidade Teste";
    private string _description = string.Empty;
    private string _imageUrl = string.Empty;
    private bool _isPublic = true;
    private int? _createdByUserId;
    private Utilizador? _createdByUser;
    private DateTime _createdAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private string _codigoConvite = "TESTE001";
    private readonly List<(Utilizador Utilizador, PapelMembroComunidade Papel)> _membros = new();

    public ComunidadeBuilder ComId(int id) { _id = id; return this; }
    public ComunidadeBuilder ComPublicId(Guid publicId) { _publicId = publicId; return this; }
    public ComunidadeBuilder ComNome(string nome) { _name = nome; return this; }
    public ComunidadeBuilder ComDescricao(string desc) { _description = desc; return this; }
    public ComunidadeBuilder Privada() { _isPublic = false; return this; }

    public ComunidadeBuilder CriadaPor(Utilizador dono)
    {
        _createdByUser = dono;
        _createdByUserId = dono.Id;
        AdicionarMembro(dono, PapelMembroComunidade.Proprietario);
        return this;
    }

    public ComunidadeBuilder ComMembro(Utilizador utilizador, PapelMembroComunidade papel = PapelMembroComunidade.Membro)
    {
        AdicionarMembro(utilizador, papel);
        return this;
    }

    private void AdicionarMembro(Utilizador utilizador, PapelMembroComunidade papel)
    {
        if (_membros.Any(m => m.Utilizador.Id == utilizador.Id))
            return;
        _membros.Add((utilizador, papel));
    }

    public Comunidade Build()
    {
        var comunidade = new Comunidade
        {
            Id = _id,
            PublicId = _publicId,
            Name = _name,
            Description = _description,
            ImageUrl = _imageUrl,
            IsPublic = _isPublic,
            CreatedByUserId = _createdByUserId,
            CreatedByUser = _createdByUser,
            CreatedAt = _createdAt,
            CodigoConvite = _codigoConvite,
        };

        foreach (var (utilizador, papel) in _membros)
        {
            comunidade.Members.Add(new ComunidadeMembro
            {
                ComunidadeId = comunidade.Id,
                Comunidade = comunidade,
                UtilizadorId = utilizador.Id,
                Utilizador = utilizador,
                Role = papel,
            });
        }

        return comunidade;
    }

    public static implicit operator Comunidade(ComunidadeBuilder builder) => builder.Build();
}
