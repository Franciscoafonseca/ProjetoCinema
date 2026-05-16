using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ValidadorFinalizacaoCompra : IValidadorFinalizacaoCompra
{
    private const int QuantidadeMaxima = 99;
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;

    public ValidadorFinalizacaoCompra(IAcessoUtilizadorRepository acessoUtilizadorRepository)
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
    }

    public async Task ValidarAsync(int utilizadorId, Carrinho? carrinho)
    {
        if (carrinho == null || !carrinho.Itens.Any())
            throw new InvalidOperationException("O carrinho esta vazio.");

        var agora = DateTime.UtcNow;
        var acessosNoCarrinho = new HashSet<int>();

        foreach (var item in carrinho.Itens)
        {
            ValidarItem(item, agora);

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

    private static void ValidarItem(CarrinhoItem item, DateTime agora)
    {
        if (item.Quantidade <= 0)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        if (item.Quantidade > QuantidadeMaxima)
            throw new InvalidOperationException(
                $"A quantidade nao pode exceder {QuantidadeMaxima}."
            );

        if (item.Acesso == null)
            throw new InvalidOperationException("Item de carrinho sem acesso associado.");

        if (!item.Acesso.IsAtivo)
            throw new InvalidOperationException(
                $"O acesso '{item.Acesso.Nome}' ja nao esta disponivel."
            );

        if (item.Acesso.Tipo != TipoAcesso.BilheteSessao && item.Quantidade != 1)
            throw new InvalidOperationException(
                "Apenas bilhetes de sessao permitem quantidade superior a 1."
            );

        switch (item.Acesso.Tipo)
        {
            case TipoAcesso.BilheteSessao:
                ValidarBilheteSessao(item.Acesso, agora);
                break;
            case TipoAcesso.PasseDiario:
                ValidarPasseDiario(item.Acesso, agora);
                break;
            case TipoAcesso.PasseCompleto:
                ValidarPasseCompleto(item.Acesso, agora);
                break;
            case TipoAcesso.AluguerDigital:
                ValidarAluguerDigital(item.Acesso);
                break;
            default:
                throw new InvalidOperationException("Tipo de acesso nao suportado no checkout.");
        }
    }

    private static void ValidarBilheteSessao(Acesso acesso, DateTime agora)
    {
        if (acesso.SessaoId == null || acesso.Sessao == null)
            throw new InvalidOperationException("Bilhete de sessao sem sessao associada.");

        if (acesso.Sessao.Fim <= agora)
            throw new InvalidOperationException("A sessao ja terminou.");
    }

    private static void ValidarPasseDiario(Acesso acesso, DateTime agora)
    {
        if (acesso.FestivalId == null || acesso.Festival == null || acesso.DataAcesso == null)
            throw new InvalidOperationException(
                "Passe diario sem festival ou data de acesso associada."
            );

        if (
            acesso.DataAcesso.Value.Date < acesso.Festival.StartDate.Date
            || acesso.DataAcesso.Value.Date > acesso.Festival.EndDate.Date
        )
            throw new InvalidOperationException(
                "A data do passe diario tem de estar dentro do periodo do festival."
            );

        if (acesso.DataAcesso.Value.Date < agora.Date)
            throw new InvalidOperationException("A data do passe diario nao pode estar no passado.");
    }

    private static void ValidarPasseCompleto(Acesso acesso, DateTime agora)
    {
        if (acesso.FestivalId == null || acesso.Festival == null)
            throw new InvalidOperationException("Passe completo sem festival associado.");

        if (acesso.Festival.EndDate < agora)
            throw new InvalidOperationException("O festival ja terminou.");
    }

    private static void ValidarAluguerDigital(Acesso acesso)
    {
        if (acesso.FilmeId == null || acesso.Filme == null)
            throw new InvalidOperationException("Aluguer digital sem filme associado.");

        if (acesso.DuracaoHoras.GetValueOrDefault(48) <= 0)
            throw new InvalidOperationException("Aluguer digital com duracao invalida.");
    }
}
