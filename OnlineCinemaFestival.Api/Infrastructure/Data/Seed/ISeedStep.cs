namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public interface ISeedStep
{
    Task ExecutarAsync(SeedContext contexto);
}
