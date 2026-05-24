using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class ComentarioBuilder
{
    private int _id = 1;
    private string _texto = "Comentario de teste.";
    private DateTime _criadoEm = new(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc);
    private bool _reportado;
    private bool _visivel = true;
    private EstadoModeracaoComentario _estadoModeracao = EstadoModeracaoComentario.Visivel;
    private int _usuarioId = 1;
    private Utilizador? _usuario;
    private int? _comunidadeId;
    private Comunidade? _comunidade;
    private int? _filmeId;
    private Filme? _filme;

    public ComentarioBuilder ComId(int id) { _id = id; return this; }
    public ComentarioBuilder ComTexto(string texto) { _texto = texto; return this; }
    public ComentarioBuilder CriadoEm(DateTime data) { _criadoEm = data; return this; }
    public ComentarioBuilder Reportado() { _reportado = true; return this; }
    public ComentarioBuilder Oculto()
    {
        _visivel = false;
        _estadoModeracao = EstadoModeracaoComentario.Oculto;
        return this;
    }

    public ComentarioBuilder DoUtilizador(Utilizador utilizador)
    {
        _usuario = utilizador;
        _usuarioId = utilizador.Id;
        return this;
    }

    public ComentarioBuilder DoUtilizador(int id) { _usuarioId = id; return this; }

    public ComentarioBuilder NaComunidade(Comunidade comunidade)
    {
        _comunidade = comunidade;
        _comunidadeId = comunidade.Id;
        return this;
    }

    public ComentarioBuilder NoFilme(Filme filme)
    {
        _filme = filme;
        _filmeId = filme.Id;
        return this;
    }

    public Comentario Build()
    {
        var comentario = new Comentario
        {
            Id = _id,
            Texto = _texto,
            CriadoEm = _criadoEm,
            Reportado = _reportado,
            Visivel = _visivel,
            EstadoModeracao = _estadoModeracao,
            UsuarioId = _usuarioId,
            ComunidadeId = _comunidadeId,
            FilmeId = _filmeId,
        };

        if (_usuario != null) comentario.Usuario = _usuario;
        if (_comunidade != null) comentario.Comunidade = _comunidade;
        if (_filme != null) comentario.Filme = _filme;

        return comentario;
    }

    public static implicit operator Comentario(ComentarioBuilder builder) => builder.Build();
}
