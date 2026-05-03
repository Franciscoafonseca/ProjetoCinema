using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CompraService
{
    private readonly List<ICompraObserver> _observadores;
    private readonly AcessoRepository _acessoRepository;

    public CompraService(AcessoRepository acessoRepository, IEnumerable<ICompraObserver> observadores)
    {
        _acessoRepository = acessoRepository;
        _observadores = observadores.ToList();
    }

    public async Task FinalizarProcessoCompra(string utilizadorId, List<CompraItemDto> itensCarrinho)
    {
        decimal valorTotal = 0;
        var acessosParaGerar = new List<Acesso>();

        // 1. Lógica de Preços
        foreach (var item in itensCarrinho)
        {
            // A Factory devolve a estratégia (Bilhete, Passe, Aluguer)
            var estrategia = AcessoStrategyFactory.GetStrategy(item.Tipo);

            var novoAcesso = new Acesso
            {
                UtilizadorId = utilizadorId,
                Tipo = item.Tipo,
                FilmeId = item.FilmeId,
                PrecoPago = estrategia.CalcularPreco(),
                DataExpiracao = estrategia.CalcularExpiracao()
            };

            valorTotal += novoAcesso.PrecoPago;
            acessosParaGerar.Add(novoAcesso);
        }

        // 2. Notificar interessados 
        foreach (var obs in _observadores)
        {
            obs.Notificar(utilizadorId, valorTotal, acessosParaGerar);
        }

        // 3. Salvar na Base de Dados
        await _acessoRepository.AddManyAsync(acessosParaGerar);
    }
}