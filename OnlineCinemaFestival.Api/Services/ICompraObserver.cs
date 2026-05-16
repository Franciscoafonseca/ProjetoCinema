using OnlineCinemaFestival.Api.Repositories;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraObserver
{
    // Este método será chamado pelo CompraService quando uma venda for concluída
    Task NotificarAsync(string utilizadorId, decimal valorTotal, List<ModelAcesso> acessos);
}