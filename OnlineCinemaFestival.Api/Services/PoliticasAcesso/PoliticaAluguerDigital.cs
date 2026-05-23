using OnlineCinemaFestival.Api.Models;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaAluguerDigital : IPoliticaAcesso
{
    public TipoAcesso TipoSuportado => TipoAcesso.AluguerDigital;

    public bool TemAcesso(ModelAcesso acesso, DateTime agora)
    {
        if (acesso.Tipo != TipoSuportado || !acesso.FilmeId.HasValue)
            return false;

        var limiteValidade = acesso.Validade
            ?? (acesso.DuracaoHoras.HasValue
                ? acesso.CriadoEm.AddHours(acesso.DuracaoHoras.Value)
                : null);

        return limiteValidade.HasValue
            && acesso.CriadoEm <= agora
            && agora <= limiteValidade.Value;
    }
}

