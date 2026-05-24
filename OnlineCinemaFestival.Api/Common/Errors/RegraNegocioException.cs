namespace OnlineCinemaFestival.Api.Common.Errors;

public class RegraNegocioException : InvalidOperationException
{
    public RegraNegocioException(string message)
        : base(message) { }
}
