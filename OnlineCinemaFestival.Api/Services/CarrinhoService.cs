using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public class CarrinhoService : ICarrinhoService
{
    private readonly Dictionary<string, List<CompraItemDto>> _carrinhos = new();

    public IReadOnlyList<CompraItemDto> ObterItens(string utilizadorId)
    {
        return _carrinhos.TryGetValue(utilizadorId, out var itens)
            ? itens
            : Array.Empty<CompraItemDto>();
    }

    public void AdicionarItem(string utilizadorId, CompraItemDto item)
    {
        if (!_carrinhos.TryGetValue(utilizadorId, out var itens))
        {
            itens = new List<CompraItemDto>();
            _carrinhos[utilizadorId] = itens;
        }

        itens.Add(item);
    }

    public bool RemoverItem(string utilizadorId, int filmeId)
    {
        if (!_carrinhos.TryGetValue(utilizadorId, out var itens))
        {
            return false;
        }

        var item = itens.FirstOrDefault(i => i.FilmeId == filmeId);
        if (item == null)
        {
            return false;
        }

        itens.Remove(item);
        return true;
    }

    public void Limpar(string utilizadorId)
    {
        _carrinhos.Remove(utilizadorId);
    }
}
