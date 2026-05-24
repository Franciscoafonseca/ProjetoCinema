using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoVisualizacaoService
{
    bool PodeVisualizar(ModelAcesso acesso);
}

