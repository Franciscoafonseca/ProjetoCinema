using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class CarrinhoRepository : ICarrinhoRepository
{
    private readonly AppDbContext _context;

    public CarrinhoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Carrinho?> GetByUtilizadorIdAsync(int utilizadorId)
    {
        return await _context
            .Carrinhos.Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Sessao)
                        .ThenInclude(s => s!.FilmesDaSessao)
                            .ThenInclude(sf => sf.Filme)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Sessao)
                        .ThenInclude(s => s!.Festival)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Festival)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Acesso)
                    .ThenInclude(a => a.Filme)
            .FirstOrDefaultAsync(c => c.UtilizadorId == utilizadorId);
    }

    public async Task<Carrinho> GetOrCreateByUtilizadorIdAsync(int utilizadorId)
    {
        var carrinho = await GetByUtilizadorIdAsync(utilizadorId);

        if (carrinho != null)
            return carrinho;

        carrinho = new Carrinho { UtilizadorId = utilizadorId, DataCriacao = DateTime.UtcNow };

        await _context.Carrinhos.AddAsync(carrinho);
        await _context.SaveChangesAsync();

        return carrinho;
    }

    public async Task<CarrinhoItem?> GetItemAsync(int carrinhoId, int itemId)
    {
        return await _context
            .ItensCarrinho.Include(i => i.Acesso)
                .ThenInclude(a => a.Sessao)
                    .ThenInclude(s => s!.FilmesDaSessao)
                        .ThenInclude(sf => sf.Filme)
            .Include(i => i.Acesso)
                .ThenInclude(a => a.Sessao)
                    .ThenInclude(s => s!.Festival)
            .Include(i => i.Acesso)
                .ThenInclude(a => a.Festival)
            .Include(i => i.Acesso)
                .ThenInclude(a => a.Filme)
            .FirstOrDefaultAsync(i => i.CarrinhoId == carrinhoId && i.Id == itemId);
    }

    public async Task<CarrinhoItem?> GetItemByAcessoAsync(int carrinhoId, int acessoId)
    {
        return await _context.ItensCarrinho.FirstOrDefaultAsync(i =>
            i.CarrinhoId == carrinhoId && i.AcessoId == acessoId
        );
    }

    public async Task<bool> ExisteItemComAcessoAsync(int carrinhoId, int acessoId)
    {
        return await _context.ItensCarrinho.AnyAsync(i =>
            i.CarrinhoId == carrinhoId && i.AcessoId == acessoId
        );
    }

    public async Task AddItemAsync(CarrinhoItem item)
    {
        await _context.ItensCarrinho.AddAsync(item);
    }

    public void RemoveItem(CarrinhoItem item)
    {
        _context.ItensCarrinho.Remove(item);
    }

    public void RemoveItems(IEnumerable<CarrinhoItem> itens)
    {
        _context.ItensCarrinho.RemoveRange(itens);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
