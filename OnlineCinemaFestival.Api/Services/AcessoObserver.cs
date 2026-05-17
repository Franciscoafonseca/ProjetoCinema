using Microsoft.Extensions.Logging;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoObserver : ICompraObserver
{
    private readonly ILogger<AcessoObserver> _logger;

    public AcessoObserver(ILogger<AcessoObserver> logger)
    {
        _logger = logger;
    }

    public Task NotificarAsync(int utilizadorId, decimal valorTotal, List<ModelAcesso> acessos)
    {
        // Regra de Negócio: Ativar os acessos (Bilhetes/Passes) comprados
        foreach (var acesso in acessos)
        {
            _logger.LogInformation("[ACESSO] Ativado: {TipoAcesso} para o utilizador {UtilizadorId}.", acesso.Tipo, utilizadorId);
        }

        return Task.CompletedTask;
    }
}
