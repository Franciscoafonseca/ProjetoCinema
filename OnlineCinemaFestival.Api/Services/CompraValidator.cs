using OnlineCinemaFestival.Api.DTOs;
namespace OnlineCinemaFestival.Api.Services;

public class CompraValidator : ICompraValidator
{
    private readonly List<ICompraItemValidator> _itemValidators;

    public CompraValidator(IEnumerable<ICompraItemValidator> itemValidators)
    {
        _itemValidators = itemValidators.ToList();
    }

    public void Validar(CompraRequest request)
    {
        if (request == null)
        {
            throw new ArgumentException("Pedido invalido.");
        }

        if (string.IsNullOrWhiteSpace(request.UtilizadorId))
        {
            throw new ArgumentException("UtilizadorId e obrigatorio.");
        }

        if (request.Itens.Count == 0)
        {
            throw new ArgumentException("O carrinho esta vazio.");
        }

        foreach (var item in request.Itens)
        {
            foreach (var validator in _itemValidators.Where(v => v.CanHandle(item.Tipo)))
            {
                validator.Validar(item);
            }
        }
    }
}
