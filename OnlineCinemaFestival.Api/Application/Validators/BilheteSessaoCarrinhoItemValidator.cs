using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Common.Errors;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public sealed class BilheteSessaoCarrinhoItemValidator : CarrinhoItemValidatorBase
{
    private readonly ISessaoRepository _sessaoRepository;
    private readonly IAcessoRepository _acessoRepository;
    private readonly TimeProvider _timeProvider;

    public BilheteSessaoCarrinhoItemValidator(
        ISessaoRepository sessaoRepository,
        IAcessoRepository acessoRepository,
        TimeProvider timeProvider
    )
    {
        _sessaoRepository = sessaoRepository;
        _acessoRepository = acessoRepository;
        _timeProvider = timeProvider;
    }

    public override TipoAcesso Tipo => TipoAcesso.BilheteSessao;

    public override bool PermiteQuantidadeMultipla => true;

    public override void ValidarPedido(CarrinhoItemCreateDTO dto)
    {
        if (dto.SessaoId is null)
            throw new RegraNegocioException("SessaoId e obrigatorio para bilhete de sessao.");

        GarantirSemAlvosExtras(dto, permiteSessao: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        var sessao = await _sessaoRepository.ObterPorIdAsync(dto.SessaoId!.Value);

        if (sessao == null)
            throw new RecursoNaoEncontradoException("Sessao nao encontrada.");

        ValidarSessao(sessao);
    }

    public override Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto)
    {
        return _acessoRepository.ObterBilheteSessaoAtivoAsync(dto.SessaoId!.Value);
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.SessaoId == null || acesso.Sessao == null)
            throw new ConflitoDominioException("Bilhete de sessao sem sessao associada.");

        ValidarSessao(acesso.Sessao);
    }

    private void ValidarSessao(Sessao sessao)
    {
        if (sessao.Fim <= _timeProvider.GetUtcNow().UtcDateTime)
            throw new ConflitoDominioException("A sessao ja terminou.");
    }
}
