using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CarrinhoService : ICarrinhoService
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IAcessoRepository _acessoRepository;
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly IReadOnlyDictionary<TipoAcesso, ICarrinhoItemValidator> _itemValidators;
    private readonly TimeProvider _timeProvider;
    private readonly int _quantidadeMaxima;

    public CarrinhoService(
        ICarrinhoRepository carrinhoRepository,
        IAcessoRepository acessoRepository,
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        IEnumerable<ICarrinhoItemValidator> itemValidators,
        TimeProvider timeProvider,
        IOptions<AcessosOptions> acessosOptions
    )
    {
        _carrinhoRepository = carrinhoRepository;
        _acessoRepository = acessoRepository;
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _itemValidators = itemValidators.ToDictionary(s => s.Tipo);
        _timeProvider = timeProvider;
        _quantidadeMaxima = acessosOptions.Value.QuantidadeMaximaCarrinho;
    }

    public async Task<CarrinhoReadDTO> ObterCarrinhoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterOuCriarPorUtilizadorIdAsync(utilizadorId);

        return CarrinhoMapper.MapToReadDTO(carrinho);
    }

    public async Task<CarrinhoReadDTO> AdicionarItemAsync(
        int utilizadorId,
        AdicionarItemCarrinhoDTO dto
    )
    {
        if (dto.AcessoId <= 0)
            throw new ArgumentException("O acesso indicado e invalido.");

        var acesso = await _acessoRepository.ObterPorIdAsync(dto.AcessoId);

        if (acesso == null)
            throw new KeyNotFoundException("Acesso nao encontrado.");

        return await AdicionarAcessoAoCarrinhoAsync(utilizadorId, acesso, dto.Quantidade);
    }

    public async Task<CarrinhoReadDTO> AdicionarItemAsync(
        int utilizadorId,
        CarrinhoItemCreateDTO dto
    )
    {
        ValidarQuantidade(dto.Quantidade);

        var validator = ObterValidator(dto.TipoAcesso);
        validator.ValidarPedido(dto);
        await validator.ValidarAlvoAsync(dto);

        var acesso = await validator.ObterAcessoAtivoAsync(dto);

        if (acesso == null)
            throw new KeyNotFoundException(
                "Nao existe um acesso ativo configurado para este item."
            );

        return await AdicionarAcessoAoCarrinhoAsync(utilizadorId, acesso, dto.Quantidade);
    }

    public async Task<CarrinhoReadDTO> AtualizarItemAsync(
        int utilizadorId,
        int itemId,
        CarrinhoItemUpdateDTO dto
    )
    {
        if (dto.Quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (dto.Quantidade > _quantidadeMaxima)
            throw new ArgumentException($"A quantidade nao pode exceder {_quantidadeMaxima}.");

        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        if (carrinho == null)
            throw new KeyNotFoundException("Carrinho nao encontrado.");

        var item = await _carrinhoRepository.ObterItemAsync(carrinho.Id, itemId);

        if (item == null)
            throw new KeyNotFoundException("Item nao encontrado no carrinho.");

        var validator = ObterValidator(item.Acesso.Tipo);

        if (!validator.PermiteQuantidadeMultipla && dto.Quantidade != 1)
            throw new InvalidOperationException(
                "Apenas bilhetes de sessao permitem quantidade superior a 1."
            );

        item.Quantidade = dto.Quantidade;
        carrinho.AtualizadoEm = Agora();

        await _carrinhoRepository.SaveChangesAsync();

        var atualizado = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);
        return CarrinhoMapper.MapToReadDTO(atualizado!);
    }

    public async Task RemoverItemAsync(int utilizadorId, int itemId)
    {
        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        if (carrinho == null)
            throw new KeyNotFoundException("Carrinho nao encontrado.");

        var item = await _carrinhoRepository.ObterItemAsync(carrinho.Id, itemId);

        if (item == null)
            throw new KeyNotFoundException("Item nao encontrado no carrinho.");

        _carrinhoRepository.RemoveItem(item);

        carrinho.AtualizadoEm = Agora();

        await _carrinhoRepository.SaveChangesAsync();
    }

    public async Task LimparCarrinhoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        if (carrinho == null)
            return;

        _carrinhoRepository.RemoveItems(carrinho.Itens);

        carrinho.AtualizadoEm = Agora();

        await _carrinhoRepository.SaveChangesAsync();
    }

    public async Task<CarrinhoValidacaoDTO> ValidarCarrinhoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);
        var resultado = new CarrinhoValidacaoDTO();

        if (carrinho == null || !carrinho.Itens.Any())
        {
            resultado.Avisos.Add("O carrinho esta vazio.");
            return resultado;
        }

        foreach (var item in carrinho.Itens)
        {
            try
            {
                ValidarAcessoParaCarrinho(item.Acesso, item.Quantidade);
                await ValidarAcessoJaCompradoAsync(utilizadorId, item.Acesso);
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                resultado.Erros.Add(
                    new CarrinhoErroValidacaoDTO
                    {
                        ItemId = item.Id,
                        Campo = item.Acesso.Tipo.ToString(),
                        Mensagem = ex.Message,
                    }
                );
            }
        }

        resultado.Total = carrinho.Itens.Sum(i => i.PrecoUnitario * i.Quantidade);

        return resultado;
    }

    public async Task<CarrinhoResumoDTO> ObterResumoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterOuCriarPorUtilizadorIdAsync(utilizadorId);
        var dto = CarrinhoMapper.MapToReadDTO(carrinho);

        return new CarrinhoResumoDTO
        {
            CarrinhoId = dto.Id,
            NumeroItens = dto.Itens.Sum(i => i.Quantidade),
            Subtotal = dto.Total,
            Itens = dto.Itens,
        };
    }

    private async Task<CarrinhoReadDTO> AdicionarAcessoAoCarrinhoAsync(
        int utilizadorId,
        Acesso acesso,
        int quantidade
    )
    {
        ValidarAcessoParaCarrinho(acesso, quantidade);
        await ValidarAcessoJaCompradoAsync(utilizadorId, acesso);

        var carrinho = await _carrinhoRepository.ObterOuCriarPorUtilizadorIdAsync(utilizadorId);
        var itemExistente = await _carrinhoRepository.ObterItemPorAcessoAsync(carrinho.Id, acesso.Id);

        if (itemExistente != null)
        {
            var validator = ObterValidator(acesso.Tipo);

            if (!validator.PermiteQuantidadeMultipla)
                throw new InvalidOperationException("Este acesso ja esta no carrinho.");

            if (itemExistente.Quantidade + quantidade > _quantidadeMaxima)
                throw new InvalidOperationException(
                    $"A quantidade total deste bilhete nao pode exceder {_quantidadeMaxima}."
                );

            itemExistente.Quantidade += quantidade;
            carrinho.AtualizadoEm = Agora();
            await _carrinhoRepository.SaveChangesAsync();

            var carrinhoComItemAtualizado = await _carrinhoRepository.ObterPorUtilizadorIdAsync(
                utilizadorId
            );
            return CarrinhoMapper.MapToReadDTO(carrinhoComItemAtualizado!);
        }

        var item = new CarrinhoItem
        {
            CarrinhoId = carrinho.Id,
            AcessoId = acesso.Id,
            PrecoUnitario = acesso.Preco,
            Quantidade = quantidade,
            DataAdicao = Agora(),
        };

        await _carrinhoRepository.AddItemAsync(item);

        carrinho.AtualizadoEm = Agora();

        await _carrinhoRepository.SaveChangesAsync();

        var carrinhoAtualizado = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        return CarrinhoMapper.MapToReadDTO(carrinhoAtualizado!);
    }

    private async Task ValidarAcessoJaCompradoAsync(int utilizadorId, Acesso acesso)
    {
        var jaPossuiAcesso = await _acessoUtilizadorRepository.ExisteAcessoAtivoAsync(
            utilizadorId,
            acesso.Id,
            Agora()
        );

        if (jaPossuiAcesso)
            throw new InvalidOperationException("O utilizador ja possui este acesso ativo.");
    }

    private void ValidarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (quantidade > _quantidadeMaxima)
            throw new ArgumentException($"A quantidade nao pode exceder {_quantidadeMaxima}.");
    }

    private ICarrinhoItemValidator ObterValidator(TipoAcesso tipo)
    {
        return _itemValidators.TryGetValue(tipo, out var validator)
            ? validator
            : throw new InvalidOperationException("Tipo de acesso nao suportado pelo carrinho.");
    }

    private void ValidarAcessoParaCarrinho(Acesso acesso, int quantidade)
    {
        if (!acesso.IsAtivo)
            throw new InvalidOperationException("Este acesso nao esta disponivel para compra.");

        ValidarQuantidade(quantidade);

        var validator = ObterValidator(acesso.Tipo);

        if (!validator.PermiteQuantidadeMultipla && quantidade != 1)
            throw new InvalidOperationException(
                "Apenas bilhetes de sessao permitem quantidade superior a 1."
            );

        validator.ValidarAcesso(acesso);
    }

    private DateTime Agora()
    {
        return _timeProvider.GetUtcNow().UtcDateTime;
    }
}
