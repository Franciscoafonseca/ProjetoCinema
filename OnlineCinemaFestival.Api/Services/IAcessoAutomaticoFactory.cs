using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoAutomaticoFactory
{
    Acesso CriarAluguerDigital(Filme filme);

    Acesso CriarPasseCompleto(Festival festival);

    Acesso CriarPasseDiario(Festival festival, DateTime dia);

    Acesso CriarBilheteSessao(Sessao sessao);
}
