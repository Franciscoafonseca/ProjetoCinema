namespace OnlineCinemaFestival.Api.Data.Seed;

public interface ISeedStep
{
    Task ExecutarAsync(SeedContext contexto);
}
