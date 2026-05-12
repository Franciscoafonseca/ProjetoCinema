using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ValidadorCheckout : IValidadorCheckout
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;

    public ValidadorCheckout(IAcessoUtilizadorRepository acessoUtilizadorRepository)
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
    }

    public async Task ValidarAsync(int utilizadorId, Carrinho? carrinho)
    {
        if (carrinho == null || !carrinho.Itens.Any())
            throw new InvalidOperationException("O carrinho está vazio.");

        var agora = DateTime.UtcNow;

        foreach (var item in carrinho.Itens)
        {
            if (!item.Acesso.IsAtivo)
            {
                throw new InvalidOperationException(
                    $"O acesso '{item.Acesso.Nome}' já não está disponível."
                );
            }

            var jaTemAcesso = await _acessoUtilizadorRepository.ExisteAcessoAtivoAsync(
                utilizadorId,
                item.AcessoId,
                agora
            );

            if (jaTemAcesso)
            {
                throw new InvalidOperationException(
                    $"O utilizador já possui um acesso ativo para '{item.Acesso.Nome}'."
                );
            }
        }
    }
}
