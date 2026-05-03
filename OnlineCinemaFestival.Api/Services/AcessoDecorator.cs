using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

// A base para todos os decoradores de acesso
public abstract class AcessoDecorator : Acesso
{
    protected Acesso _acesso;
    protected AcessoDecorator(Acesso acesso) => _acesso = acesso;
    public abstract bool TemAcesso();
}

// Implementação específica para a regra de 48 horas
public class Aluguer48hDecorator : AcessoDecorator
{
    public Aluguer48hDecorator(Acesso acesso) : base(acesso) { }

    public override bool TemAcesso()
    {
        // Regra: Verifica se ainda está dentro das 48h desde a criação
        return DateTime.UtcNow <= _acesso.DataCriacao.AddHours(48);
    }
}