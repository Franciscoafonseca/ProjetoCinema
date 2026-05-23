using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

// A base para todos os decoradores de acesso
public abstract class AcessoDecorator : ModelAcesso
{
    protected ModelAcesso _acesso;
    protected AcessoDecorator(ModelAcesso acesso) => _acesso = acesso;
    public abstract bool TemAcesso();
}

// Implementacao especifica para a validade configurada do aluguer digital.
public class AluguerDigitalDecorator : AcessoDecorator
{
    public AluguerDigitalDecorator(ModelAcesso acesso) : base(acesso) { }

    public override bool TemAcesso()
    {
        // Regra: Verifica se ainda está dentro da validade
        if (_acesso.Validade == null)
        {
            return true;
        }

        return DateTime.UtcNow <= _acesso.Validade.Value;
    }
}
