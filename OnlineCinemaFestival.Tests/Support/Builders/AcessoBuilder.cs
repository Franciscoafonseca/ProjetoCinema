using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class AcessoBuilder
{
    private int _id = 1;
    private string _nome = "Acesso";
    private TipoAcesso _tipo = TipoAcesso.BilheteSessao;
    private decimal _preco = 10m;
    private bool _isAtivo = true;
    private DateTime _criadoEm = new(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
    private int? _sessaoId;
    private Sessao? _sessao;
    private int? _festivalId;
    private Festival? _festival;
    private int? _filmeId;
    private Filme? _filme;
    private DateTime? _dataAcesso;
    private int? _duracaoHoras;

    public AcessoBuilder ComId(int id) { _id = id; return this; }
    public AcessoBuilder ComNome(string nome) { _nome = nome; return this; }
    public AcessoBuilder ComPreco(decimal preco) { _preco = preco; return this; }
    public AcessoBuilder Inativo() { _isAtivo = false; return this; }

    public AcessoBuilder ComoBilheteSessao(Sessao sessao)
    {
        _tipo = TipoAcesso.BilheteSessao;
        _sessao = sessao;
        _sessaoId = sessao.Id;
        _filmeId = sessao.FilmeId;
        return this;
    }

    public AcessoBuilder ComoPasseDiario(Festival festival, DateTime data)
    {
        _tipo = TipoAcesso.PasseDiario;
        _festival = festival;
        _festivalId = festival.Id;
        _dataAcesso = data;
        return this;
    }

    public AcessoBuilder ComoPasseCompleto(Festival festival)
    {
        _tipo = TipoAcesso.PasseCompleto;
        _festival = festival;
        _festivalId = festival.Id;
        return this;
    }

    public AcessoBuilder ComoAluguerDigital(Filme filme, int duracaoHoras = 48)
    {
        _tipo = TipoAcesso.AluguerDigital;
        _filme = filme;
        _filmeId = filme.Id;
        _duracaoHoras = duracaoHoras;
        return this;
    }

    public Acesso Build()
    {
        var acesso = new Acesso
        {
            Id = _id,
            Nome = _nome,
            Tipo = _tipo,
            Preco = _preco,
            IsAtivo = _isAtivo,
            CriadoEm = _criadoEm,
            SessaoId = _sessaoId,
            FestivalId = _festivalId,
            FilmeId = _filmeId,
            DataAcesso = _dataAcesso,
            DuracaoHoras = _duracaoHoras,
        };

        if (_sessao != null) acesso.Sessao = _sessao;
        if (_festival != null) acesso.Festival = _festival;
        if (_filme != null) acesso.Filme = _filme;

        return acesso;
    }

    public static implicit operator Acesso(AcessoBuilder builder) => builder.Build();
}
