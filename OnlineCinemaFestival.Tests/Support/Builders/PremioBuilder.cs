using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class PremioBuilder
{
    private int _id = 1;
    private int _festivalId = 1;
    private Festival? _festival;
    private string _nome = "Premio do publico";
    private string _descricao = string.Empty;
    private DateTime _dataAbertura = new(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
    private DateTime _dataFecho = new(2026, 5, 7, 0, 0, 0, DateTimeKind.Utc);
    private EstadoPremio _estado = EstadoPremio.Aberto;

    public PremioBuilder ComId(int id) { _id = id; return this; }
    public PremioBuilder ComNome(string nome) { _nome = nome; return this; }
    public PremioBuilder ComDescricao(string desc) { _descricao = desc; return this; }

    public PremioBuilder NoFestival(Festival festival)
    {
        _festival = festival;
        _festivalId = festival.Id;
        return this;
    }

    public PremioBuilder NoFestival(int id) { _festivalId = id; return this; }

    public PremioBuilder ComVotacao(DateTime inicio, DateTime fim)
    {
        _dataAbertura = inicio;
        _dataFecho = fim;
        return this;
    }

    public PremioBuilder NoEstado(EstadoPremio estado) { _estado = estado; return this; }

    public PremioFestival Build()
    {
        var premio = new PremioFestival
        {
            Id = _id,
            FestivalId = _festivalId,
            Nome = _nome,
            Descricao = _descricao,
            DataAberturaVotacao = _dataAbertura,
            DataFechoVotacao = _dataFecho,
            EstadoPremio = _estado,
        };

        if (_festival != null) premio.Festival = _festival;

        return premio;
    }

    public static implicit operator PremioFestival(PremioBuilder builder) => builder.Build();
}
