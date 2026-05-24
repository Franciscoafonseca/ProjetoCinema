namespace OnlineCinemaFestival.Api.Common.Errors;

public class ConflitoDominioException : InvalidOperationException
{
    public ConflitoDominioException(string message)
        : base(message) { }
}
