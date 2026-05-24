namespace OnlineCinemaFestival.Api.Excecoes;

public class ConflitoDominioException : InvalidOperationException
{
    public ConflitoDominioException(string message)
        : base(message) { }
}
