using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.Acesso;

public interface IEstrategiaValidacaoAcesso
{
    TipoAcesso Tipo { get; }

    string Nome { get; }

    string Descricao { get; }

    Task ValidarAsync(AcessoCreateDto dto);
}
