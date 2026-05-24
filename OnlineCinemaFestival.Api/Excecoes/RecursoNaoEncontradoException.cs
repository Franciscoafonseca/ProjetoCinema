namespace OnlineCinemaFestival.Api.Excecoes;

public class RecursoNaoEncontradoException : KeyNotFoundException
{
    public RecursoNaoEncontradoException(string message)
        : base(message) { }
}
