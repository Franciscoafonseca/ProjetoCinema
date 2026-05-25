using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services.ValidacaoAcesso;

public interface IEstrategiaValidacaoAcesso
{
    TipoAcesso Tipo { get; }

    string Nome { get; }

    string Descricao { get; }

    Task ValidarAsync(AcessoCreateDTO dto);
}
