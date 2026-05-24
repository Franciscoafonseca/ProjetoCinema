namespace OnlineCinemaFestival.Api.Common.Errors;

public class RecursoNaoEncontradoException : KeyNotFoundException
{
    public RecursoNaoEncontradoException(string message)
        : base(message) { }
}
