using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class FestivalBuilder
{
    private int _id = 1;
    private string _name = "Festival Teste";
    private string _description = string.Empty;
    private DateTime _startDate = new(2026, 5, 1);
    private DateTime _endDate = new(2026, 5, 7);
    private string? _premios;

    public FestivalBuilder ComId(int id) { _id = id; return this; }
    public FestivalBuilder ComNome(string nome) { _name = nome; return this; }
    public FestivalBuilder ComDescricao(string descricao) { _description = descricao; return this; }
    public FestivalBuilder Entre(DateTime inicio, DateTime fim) { _startDate = inicio; _endDate = fim; return this; }
    public FestivalBuilder ComPremios(string premios) { _premios = premios; return this; }

    public Festival Build() => new()
    {
        Id = _id,
        Name = _name,
        Description = _description,
        StartDate = _startDate,
        EndDate = _endDate,
        Premios = _premios,
    };

    public static implicit operator Festival(FestivalBuilder builder) => builder.Build();
}
