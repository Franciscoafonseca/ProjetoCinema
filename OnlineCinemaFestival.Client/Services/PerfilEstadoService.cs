using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class PerfilEstadoService
{
    public event Action? Alterado;

    public string Nome { get; private set; } = string.Empty;

    public string FotoUrl { get; private set; } = string.Empty;

    public bool TemPerfil => !string.IsNullOrWhiteSpace(Nome) || !string.IsNullOrWhiteSpace(FotoUrl);

    public void Atualizar(PerfilPublicoDTO? perfil)
    {
        Nome = perfil?.Name ?? string.Empty;
        FotoUrl = perfil?.ProfileImageUrl ?? string.Empty;
        Alterado?.Invoke();
    }

    public void Limpar()
    {
        Nome = string.Empty;
        FotoUrl = string.Empty;
        Alterado?.Invoke();
    }
}
