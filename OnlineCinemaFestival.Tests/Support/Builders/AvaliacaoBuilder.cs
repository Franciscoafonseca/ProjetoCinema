using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class AvaliacaoBuilder
{
    private int _id = 1;
    private int _filmeId = 1;
    private Filme? _filme;
    private int _usuarioId = 1;
    private Utilizador? _usuario;
    private int _pontuacao = 8;
    private string _texto = string.Empty;
    private DateTime _data = new(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc);

    public AvaliacaoBuilder ComId(int id) { _id = id; return this; }

    public AvaliacaoBuilder DoFilme(Filme filme)
    {
        _filme = filme;
        _filmeId = filme.Id;
        return this;
    }

    public AvaliacaoBuilder DoFilme(int id) { _filmeId = id; return this; }

    public AvaliacaoBuilder DoUtilizador(Utilizador utilizador)
    {
        _usuario = utilizador;
        _usuarioId = utilizador.Id;
        return this;
    }

    public AvaliacaoBuilder DoUtilizador(int id) { _usuarioId = id; return this; }

    public AvaliacaoBuilder ComPontuacao(int pontuacao) { _pontuacao = pontuacao; return this; }
    public AvaliacaoBuilder ComTexto(string texto) { _texto = texto; return this; }
    public AvaliacaoBuilder Em(DateTime data) { _data = data; return this; }

    public Avaliacao Build()
    {
        var avaliacao = new Avaliacao
        {
            Id = _id,
            FilmeId = _filmeId,
            UsuarioId = _usuarioId,
            Pontuacao = _pontuacao,
            Texto = _texto,
            Data = _data,
        };

        if (_filme != null) avaliacao.Filme = _filme;
        if (_usuario != null) avaliacao.Usuario = _usuario;

        return avaliacao;
    }

    public static implicit operator Avaliacao(AvaliacaoBuilder builder) => builder.Build();
}
