using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class FilmeBuilder
{
    private int _id = 1;
    private string _titulo = "Filme Teste";
    private string _sinopse = string.Empty;
    private DateTime _dataLancamento = new(2024, 1, 1);
    private int? _duracaoMinutos = 120;
    private double? _avaliacaoTmdb;
    private string _capaUrl = "/poster.jpg";
    private string? _videoUrl = "/videos/teste.mp4";
    private string? _trailerUrl;
    private readonly List<Genero> _generos = new();
    private readonly List<Avaliacao> _avaliacoes = new();
    private readonly List<Visualizacao> _visualizacoes = new();

    public FilmeBuilder ComId(int id) { _id = id; return this; }
    public FilmeBuilder ComTitulo(string titulo) { _titulo = titulo; return this; }
    public FilmeBuilder ComSinopse(string sinopse) { _sinopse = sinopse; return this; }
    public FilmeBuilder LancadoEm(DateTime data) { _dataLancamento = data; return this; }
    public FilmeBuilder ComDuracao(int minutos) { _duracaoMinutos = minutos; return this; }
    public FilmeBuilder ComAvaliacaoTmdb(double valor) { _avaliacaoTmdb = valor; return this; }
    public FilmeBuilder ComCapa(string url) { _capaUrl = url; return this; }
    public FilmeBuilder ComVideo(string url) { _videoUrl = url; return this; }
    public FilmeBuilder ComTrailer(string url) { _trailerUrl = url; return this; }

    public FilmeBuilder ComGenero(params Genero[] generos)
    {
        _generos.AddRange(generos);
        return this;
    }

    public FilmeBuilder ComAvaliacoes(params int[] pontuacoes)
    {
        foreach (var pontuacao in pontuacoes)
            _avaliacoes.Add(new Avaliacao { FilmeId = _id, Pontuacao = pontuacao });
        return this;
    }

    public FilmeBuilder ComVisualizacoes(int quantidade)
    {
        for (var i = 0; i < quantidade; i++)
            _visualizacoes.Add(new Visualizacao { Id = _visualizacoes.Count + 1, FilmeId = _id });
        return this;
    }

    public Filme Build()
    {
        var filme = new Filme
        {
            Id = _id,
            Titulo = _titulo,
            Sinopse = _sinopse,
            DataLancamento = _dataLancamento,
            DuracaoMinutos = _duracaoMinutos,
            AvaliacaoTmdb = _avaliacaoTmdb,
            CapaUrl = _capaUrl,
            VideoUrl = _videoUrl,
            TrailerUrl = _trailerUrl,
        };

        foreach (var genero in _generos)
        {
            filme.FilmeGeneros.Add(new FilmeGenero
            {
                FilmeId = _id,
                Filme = filme,
                Genero = genero,
                GeneroId = genero.Id,
            });
        }

        foreach (var avaliacao in _avaliacoes)
            filme.Avaliacoes.Add(avaliacao);

        foreach (var visualizacao in _visualizacoes)
            filme.Visualizacoes.Add(visualizacao);

        return filme;
    }

    public static implicit operator Filme(FilmeBuilder builder) => builder.Build();
}
