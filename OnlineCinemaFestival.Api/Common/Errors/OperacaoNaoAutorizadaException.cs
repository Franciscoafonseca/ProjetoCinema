namespace OnlineCinemaFestival.Api.Common.Errors;

public class OperacaoNaoAutorizadaException : UnauthorizedAccessException
{
    public OperacaoNaoAutorizadaException(string message)
        : base(message) { }
}
