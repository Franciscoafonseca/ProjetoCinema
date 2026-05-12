using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CarrinhoService : ICarrinhoService
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IAcessoRepository _acessoRepository;

    public CarrinhoService(
        ICarrinhoRepository carrinhoRepository,
        IAcessoRepository acessoRepository
    )
    {
        _carrinhoRepository = carrinhoRepository;
        _acessoRepository = acessoRepository;
    }

    public async Task<CarrinhoReadDto> ObterCarrinhoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.GetOrCreateByUtilizadorIdAsync(utilizadorId);

        return CarrinhoMapper.MapToReadDto(carrinho);
    }

    public async Task<CarrinhoReadDto> AdicionarItemAsync(
        int utilizadorId,
        AdicionarItemCarrinhoDto dto
    )
    {
        if (dto.AcessoId <= 0)
            throw new ArgumentException("O acesso indicado é inválido.");

        var acesso = await _acessoRepository.GetByIdAsync(dto.AcessoId);

        if (acesso == null)
            throw new KeyNotFoundException("Acesso não encontrado.");

        if (!acesso.IsAtivo)
            throw new InvalidOperationException("Este acesso não está disponível para compra.");

        var carrinho = await _carrinhoRepository.GetOrCreateByUtilizadorIdAsync(utilizadorId);

        var jaExiste = await _carrinhoRepository.ExisteItemComAcessoAsync(carrinho.Id, acesso.Id);

        if (jaExiste)
            throw new InvalidOperationException("Este acesso já está no carrinho.");

        var item = new ItemCarrinho
        {
            CarrinhoId = carrinho.Id,
            AcessoId = acesso.Id,
            PrecoUnitario = acesso.Preco,
            Quantidade = 1,
            DataAdicao = DateTime.UtcNow,
        };

        await _carrinhoRepository.AddItemAsync(item);

        carrinho.AtualizadoEm = DateTime.UtcNow;

        await _carrinhoRepository.SaveChangesAsync();

        var carrinhoAtualizado = await _carrinhoRepository.GetByUtilizadorIdAsync(utilizadorId);

        return CarrinhoMapper.MapToReadDto(carrinhoAtualizado!);
    }

    public async Task RemoverItemAsync(int utilizadorId, int itemId)
    {
        var carrinho = await _carrinhoRepository.GetByUtilizadorIdAsync(utilizadorId);

        if (carrinho == null)
            throw new KeyNotFoundException("Carrinho não encontrado.");

        var item = await _carrinhoRepository.GetItemAsync(carrinho.Id, itemId);

        if (item == null)
            throw new KeyNotFoundException("Item não encontrado no carrinho.");

        _carrinhoRepository.RemoveItem(item);

        carrinho.AtualizadoEm = DateTime.UtcNow;

        await _carrinhoRepository.SaveChangesAsync();
    }

    public async Task LimparCarrinhoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.GetByUtilizadorIdAsync(utilizadorId);

        if (carrinho == null)
            return;

        _carrinhoRepository.RemoveItems(carrinho.Itens);

        carrinho.AtualizadoEm = DateTime.UtcNow;

        await _carrinhoRepository.SaveChangesAsync();
    }
}
