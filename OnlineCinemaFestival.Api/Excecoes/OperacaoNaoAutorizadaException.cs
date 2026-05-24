namespace OnlineCinemaFestival.Api.Excecoes;

public class OperacaoNaoAutorizadaException : UnauthorizedAccessException
{
    public OperacaoNaoAutorizadaException(string message)
        : base(message) { }
}
