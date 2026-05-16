using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Repositories;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

public class CompraService : ICompraService
{
    private readonly List<ICompraObserver> _observadores;
    private readonly List<IPrecoStrategy> _precoStrategies;
    private readonly IAcessoRepository _acessoRepository;
    private readonly IAcessoFactory _acessoFactory;

    public CompraService(
        IAcessoRepository acessoRepository,
        IEnumerable<ICompraObserver> observadores,
        IEnumerable<IPrecoStrategy> precoStrategies,
        IAcessoFactory acessoFactory)
    {
        _acessoRepository = acessoRepository;
        _observadores = observadores.ToList();
        _precoStrategies = precoStrategies.ToList();
        _acessoFactory = acessoFactory;
    }

    public async Task<CompraResultado> FinalizarProcessoCompra(string utilizadorId, List<CompraItemDto> itensCarrinho)
    {
        decimal valorTotal = 0;
        var acessosParaGerar = new List<ModelAcesso>();
        var itensHistorico = new List<CompraHistoricoItemDto>();

        // 1. Lógica de Preços
        foreach (var item in itensCarrinho)
        {
            var estrategia = _precoStrategies.FirstOrDefault(s => s.CanHandle(item.Tipo));

            if (estrategia == null)
            {
                throw new InvalidOperationException("Nao existe estrategia de preco para este tipo de acesso.");
            }

            var preco = estrategia.CalcularPreco(item);

            var validade = estrategia.CalcularValidade(item);
            var novoAcesso = _acessoFactory.Criar(
                utilizadorId,
                item,
                preco,
                validade);

            valorTotal += preco;
            acessosParaGerar.Add(novoAcesso);
            itensHistorico.Add(new CompraHistoricoItemDto
            {
                Tipo = item.Tipo,
                FilmeId = item.FilmeId,
                SessaoId = item.SessaoId,
                PrecoPago = preco,
                Validade = validade
            });
        }

        // 2. Notificar interessados 
        var notificacoes = _observadores
            .Select(obs => obs.NotificarAsync(utilizadorId, valorTotal, acessosParaGerar));
        await Task.WhenAll(notificacoes);

        // 3. Salvar na Base de Dados
        await _acessoRepository.AddManyAsync(acessosParaGerar);
        await _acessoRepository.SaveChangesAsync();

        return new CompraResultado
        {
            Total = valorTotal,
            PontosGanhos = (int)(valorTotal / 10),
            Itens = itensHistorico
        };
    }

}