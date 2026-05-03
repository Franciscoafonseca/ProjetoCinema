using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public interface ICompraObserver
{
    // Este método será chamado pelo CompraService quando uma venda for concluída
    void Notificar(string utilizadorId, decimal valorTotal, List<Acesso> acessos);
}