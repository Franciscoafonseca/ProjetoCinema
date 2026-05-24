using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Excecoes;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ValidadorFinalizacaoCompra : IValidadorFinalizacaoCompra
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly IReadOnlyDictionary<TipoAcesso, ICarrinhoItemValidator> _itemValidators;
    private readonly TimeProvider _timeProvider;
    private readonly int _quantidadeMaxima;

    public ValidadorFinalizacaoCompra(
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        IEnumerable<ICarrinhoItemValidator> itemValidators,
        TimeProvider timeProvider,
        IOptions<AcessosOptions> acessosOptions
    )
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _itemValidators = itemValidators.ToDictionary(s => s.Tipo);
        _timeProvider = timeProvider;
        _quantidadeMaxima = acessosOptions.Value.QuantidadeMaximaCarrinho;
    }

    public async Task ValidarAsync(int utilizadorId, Carrinho? carrinho)
    {
        if (carrinho == null || !carrinho.Itens.Any())
            throw new RegraNegocioException("O carrinho esta vazio.");

        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        var acessosNoCarrinho = new HashSet<int>();

        foreach (var item in carrinho.Itens)
        {
            ValidarItem(item);

            if (!acessosNoCarrinho.Add(item.AcessoId))
                throw new ConflitoDominioException(
                    $"O acesso '{item.Acesso.Nome}' esta duplicado no carrinho."
                );

            var jaTemAcesso = await _acessoUtilizadorRepository.ExisteAcessoAtivoAsync(
                utilizadorId,
                item.AcessoId,
                agora
            );

            if (jaTemAcesso)
            {
                throw new ConflitoDominioException(
                    $"O utilizador ja possui um acesso ativo para '{item.Acesso.Nome}'."
                );
            }
        }
    }

    private void ValidarItem(CarrinhoItem item)
    {
        if (item.Quantidade <= 0)
            throw new RegraNegocioException("A quantidade deve ser maior que zero.");

        if (item.Quantidade > _quantidadeMaxima)
            throw new RegraNegocioException(
                $"A quantidade nao pode exceder {_quantidadeMaxima}."
            );

        if (item.Acesso == null)
            throw new ConflitoDominioException("Item de carrinho sem acesso associado.");

        if (!item.Acesso.IsAtivo)
            throw new ConflitoDominioException(
                $"O acesso '{item.Acesso.Nome}' ja nao esta disponivel."
            );

        var validator = ObterValidator(item.Acesso.Tipo);

        if (!validator.PermiteQuantidadeMultipla && item.Quantidade != 1)
            throw new RegraNegocioException(
                "Apenas bilhetes de sessao permitem quantidade superior a 1."
            );

        validator.ValidarAcesso(item.Acesso);
    }

    private ICarrinhoItemValidator ObterValidator(TipoAcesso tipo)
    {
        return _itemValidators.TryGetValue(tipo, out var validator)
            ? validator
            : throw new RegraNegocioException("Tipo de acesso nao suportado no checkout.");
    }
}
