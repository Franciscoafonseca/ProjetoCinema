namespace OnlineCinemaFestival.Api.Excecoes;

public class RegraNegocioException : InvalidOperationException
{
    public RegraNegocioException(string message)
        : base(message) { }
}
