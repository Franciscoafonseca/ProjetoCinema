namespace OnlineCinemaFestival.Api.Services;

public class PublicacaoPremiosBackgroundService : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(15);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PublicacaoPremiosBackgroundService> _logger;

    public PublicacaoPremiosBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<PublicacaoPremiosBackgroundService> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Intervalo);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await PublicarPendentesAsync(stoppingToken);
        }
    }

    private async Task PublicarPendentesAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IPublicacaoPremiosService>();
            var publicados = await service.PublicarResultadosPendentesAsync(stoppingToken);

            if (publicados > 0)
                _logger.LogInformation("{Quantidade} resultados de premios publicados.", publicados);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Publicacao automatica de premios falhou.");
        }
    }
}
