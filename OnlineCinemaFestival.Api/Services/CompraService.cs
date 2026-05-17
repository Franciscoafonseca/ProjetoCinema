using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CompraService : ICompraService
{
    private readonly ICompraRepository _compraRepository;
    private readonly List<ICompraObserver> _observadores;
    private readonly List<IPrecoStrategy> _precoStrategies;
    private readonly IAcessoRepository _acessoRepository;
    private readonly IAcessoFactory _acessoFactory;

    public CompraService(
        ICompraRepository compraRepository,
        IAcessoRepository acessoRepository,
        IEnumerable<ICompraObserver> observadores,
        IEnumerable<IPrecoStrategy> precoStrategies,
        IAcessoFactory acessoFactory
    )
    {
        _compraRepository = compraRepository;
        _acessoRepository = acessoRepository;
        _observadores = observadores.ToList();
        _precoStrategies = precoStrategies.ToList();
        _acessoFactory = acessoFactory;
    }

    public async Task<IEnumerable<CompraReadDTO>> ObterComprasDoUtilizadorAsync(int utilizadorId)
    {
        var compras = await _compraRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        return compras.Select(CompraMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<CompraHistoricoReadDto>> ObterHistoricoDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var compras = await _compraRepository.ObterHistoricoPorUtilizadorAsync(utilizadorId);

        return compras.Select(c => new CompraHistoricoReadDto
        {
            Id = c.Id,
            UtilizadorId = c.UtilizadorId,
            Data = c.CriadaEm,
            Total = c.ValorTotal,
            PontosGanhos = (int)(c.ValorTotal / 10),
            Itens = c
                .Itens.Select(i => new CompraHistoricoItemReadDto
                {
                    Id = i.Id,
                    FilmeId = i.Acesso.FilmeId,
                    SessaoId = i.Acesso.SessaoId,
                    TipoAcesso = (int)i.TipoAcesso,
                    PrecoPago = i.Subtotal,
                    Validade = null,
                })
                .ToList(),
        });
    }

    public async Task<CompraResultado> FinalizarProcessoCompraAsync(
        int utilizadorId,
        List<CompraItemDto> itensCarrinho
    )
    {
        decimal valorTotal = 0;
        var acessosParaGerar = new List<Acesso>();
        var itensHistorico = new List<CompraHistoricoItemDto>();

        foreach (var item in itensCarrinho)
        {
            var estrategia = _precoStrategies.FirstOrDefault(s => s.CanHandle(item.Tipo));

            if (estrategia == null)
                throw new InvalidOperationException(
                    "Não existe estratégia de preço para este tipo de acesso."
                );

            var preco = estrategia.CalcularPreco(item);
            var validade = estrategia.CalcularValidade(item);
            var novoAcesso = _acessoFactory.Criar(utilizadorId, item, preco, validade);

            valorTotal += preco;
            acessosParaGerar.Add(novoAcesso);
            itensHistorico.Add(new CompraHistoricoItemDto
            {
                Tipo = item.Tipo,
                FilmeId = item.FilmeId,
                SessaoId = item.SessaoId,
                PrecoPago = preco,
                Validade = validade,
            });
        }

        var notificacoes = _observadores.Select(obs =>
            obs.NotificarAsync(utilizadorId, valorTotal, acessosParaGerar)
        );
        await Task.WhenAll(notificacoes);

        await _acessoRepository.AddManyAsync(acessosParaGerar);
        await _acessoRepository.SaveChangesAsync();

        return new CompraResultado
        {
            Total = valorTotal,
            PontosGanhos = (int)(valorTotal / 10),
            Itens = itensHistorico,
        };
    }
}
