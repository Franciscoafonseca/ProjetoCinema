using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ValidadorFinalizacaoCompra : IValidadorFinalizacaoCompra
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly IReadOnlyDictionary<TipoAcesso, ICarrinhoAcessoStrategy> _acessoStrategies;
    private readonly int _quantidadeMaxima;

    public ValidadorFinalizacaoCompra(
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        IEnumerable<ICarrinhoAcessoStrategy> acessoStrategies,
        IConfiguration configuration
    )
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _acessoStrategies = acessoStrategies.ToDictionary(s => s.Tipo);
        _quantidadeMaxima = AcessosConfiguracao.ObterQuantidadeMaximaCarrinho(configuration);
    }

    public async Task ValidarAsync(int utilizadorId, Carrinho? carrinho)
    {
        if (carrinho == null || !carrinho.Itens.Any())
            throw new InvalidOperationException("O carrinho esta vazio.");

        var agora = DateTime.UtcNow;
        var acessosNoCarrinho = new HashSet<int>();

        foreach (var item in carrinho.Itens)
        {
            ValidarItem(item);

            if (!acessosNoCarrinho.Add(item.AcessoId))
                throw new InvalidOperationException(
                    $"O acesso '{item.Acesso.Nome}' esta duplicado no carrinho."
                );

            var jaTemAcesso = await _acessoUtilizadorRepository.ExisteAcessoAtivoAsync(
                utilizadorId,
                item.AcessoId,
                agora
            );

            if (jaTemAcesso)
            {
                throw new InvalidOperationException(
                    $"O utilizador ja possui um acesso ativo para '{item.Acesso.Nome}'."
                );
            }
        }
    }

    private void ValidarItem(CarrinhoItem item)
    {
        if (item.Quantidade <= 0)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        if (item.Quantidade > _quantidadeMaxima)
            throw new InvalidOperationException(
                $"A quantidade nao pode exceder {_quantidadeMaxima}."
            );

        if (item.Acesso == null)
            throw new InvalidOperationException("Item de carrinho sem acesso associado.");

        if (!item.Acesso.IsAtivo)
            throw new InvalidOperationException(
                $"O acesso '{item.Acesso.Nome}' ja nao esta disponivel."
            );

        var strategy = ObterStrategy(item.Acesso.Tipo);

        if (!strategy.PermiteQuantidadeMultipla && item.Quantidade != 1)
            throw new InvalidOperationException(
                "Apenas bilhetes de sessao permitem quantidade superior a 1."
            );

        strategy.ValidarAcesso(item.Acesso);
    }

    private ICarrinhoAcessoStrategy ObterStrategy(TipoAcesso tipo)
    {
        return _acessoStrategies.TryGetValue(tipo, out var strategy)
            ? strategy
            : throw new InvalidOperationException("Tipo de acesso nao suportado no checkout.");
    }
}
