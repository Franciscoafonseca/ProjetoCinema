using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class SessaoBuilder
{
    private int _id = 1;
    private int _festivalId = 1;
    private Festival? _festival;
    private int _filmeId = 1;
    private Filme? _filme;
    private TipoSessao _tipo = TipoSessao.HorarioFixo;
    private DateTime _inicio = new(2026, 5, 24, 20, 0, 0, DateTimeKind.Utc);
    private DateTime _fim = new(2026, 5, 24, 22, 0, 0, DateTimeKind.Utc);
    private bool _temChatAoVivo = true;
    private string? _observacoes;

    public SessaoBuilder ComId(int id) { _id = id; return this; }

    public SessaoBuilder NoFestival(Festival festival)
    {
        _festival = festival;
        _festivalId = festival.Id;
        return this;
    }

    public SessaoBuilder NoFestival(int festivalId) { _festivalId = festivalId; return this; }

    public SessaoBuilder DoFilme(Filme filme)
    {
        _filme = filme;
        _filmeId = filme.Id;
        return this;
    }

    public SessaoBuilder DoFilme(int filmeId) { _filmeId = filmeId; return this; }

    public SessaoBuilder DoTipo(TipoSessao tipo) { _tipo = tipo; return this; }
    public SessaoBuilder Entre(DateTime inicio, DateTime fim) { _inicio = inicio; _fim = fim; return this; }
    public SessaoBuilder ComChatAoVivo() { _temChatAoVivo = true; return this; }
    public SessaoBuilder SemChatAoVivo() { _temChatAoVivo = false; return this; }
    public SessaoBuilder ComObservacoes(string observacoes) { _observacoes = observacoes; return this; }

    public Sessao Build()
    {
        var sessao = new Sessao
        {
            Id = _id,
            FestivalId = _festivalId,
            FilmeId = _filmeId,
            Tipo = _tipo,
            Inicio = _inicio,
            Fim = _fim,
            TemChatAoVivo = _temChatAoVivo,
            Observacoes = _observacoes,
        };

        if (_festival != null) sessao.Festival = _festival;
        if (_filme != null) sessao.Filme = _filme;

        return sessao;
    }

    public static implicit operator Sessao(SessaoBuilder builder) => builder.Build();
}
