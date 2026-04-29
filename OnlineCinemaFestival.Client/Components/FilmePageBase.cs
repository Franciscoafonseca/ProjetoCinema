using Microsoft.AspNetCore.Components;

namespace OnlineCinemaFestival.Client.Components;

public abstract class FilmePageBase : ComponentBase
{
    protected bool _loading = true;
    protected string? _erro;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await CarregarDadosAsync();
        }
        catch
        {
            _erro = "Erro ao carregar dados. Tente novamente.";
        }
        finally
        {
            _loading = false;
        }
    }

    protected abstract Task CarregarDadosAsync();
}