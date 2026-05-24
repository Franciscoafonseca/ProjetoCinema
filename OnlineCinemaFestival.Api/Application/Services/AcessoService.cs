using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Common.Errors;
using OnlineCinemaFestival.Api.Mapping;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services.AcessosFolder;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoService : IAcessoService
{
    private readonly IAcessoRepository _repository;
    private readonly IValidacaoAcessoStrategyFactory _fabricaValidacao;
    private readonly int _duracaoAluguerDigitalHoras;

    public AcessoService(
        IAcessoRepository repository,
        IValidacaoAcessoStrategyFactory fabricaValidacao,
        IOptions<AcessosOptions> acessosOptions
    )
    {
        _repository = repository;
        _fabricaValidacao = fabricaValidacao;
        _duracaoAluguerDigitalHoras = acessosOptions.Value.DuracaoAluguerDigitalHoras;
    }

    public async Task<IEnumerable<AcessoReadDTO>> ObterTodosAsync()
    {
        var acessos = await _repository.ObterTodosAsync();

        return acessos.Select(AcessoMapper.MapToReadDTO);
    }

    public async Task<AcessoReadDTO?> ObterPorIdAsync(int id)
    {
        var acesso = await _repository.ObterPorIdAsync(id);

        if (acesso == null)
            return null;

        return AcessoMapper.MapToReadDTO(acesso);
    }

    public IEnumerable<TipoAcessoReadDTO> GetTiposAcesso()
    {
        return _fabricaValidacao
            .ObterTodas()
            .Select(estrategia => new TipoAcessoReadDTO
            {
                Tipo = estrategia.Tipo,
                Nome = estrategia.Nome,
                Descricao = estrategia.Descricao,
            });
    }

    public async Task<AcessoReadDTO> CriarAsync(AcessoCreateDTO dto)
    {
        ValidateCommonData(dto.Nome, dto.Preco);

        var estrategia = _fabricaValidacao.ObterEstrategia(dto.Tipo);

        await estrategia.ValidarAsync(dto);

        var acesso = AcessoMapper.MapFromCreateDTO(dto, _duracaoAluguerDigitalHoras);

        await _repository.AddAsync(acesso);
        await _repository.SaveChangesAsync();

        var created = await _repository.ObterPorIdAsync(acesso.Id);

        return AcessoMapper.MapToReadDTO(created!);
    }

    public async Task AtualizarAsync(int id, AcessoUpdateDTO dto)
    {
        ValidateCommonData(dto.Nome, dto.Preco);

        var acesso = await _repository.ObterPorIdAsync(id);

        if (acesso == null)
            throw new RecursoNaoEncontradoException("Acesso nao encontrado.");

        AcessoMapper.MapToExistingAcesso(dto, acesso);

        await _repository.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var acesso = await _repository.ObterPorIdAsync(id);

        if (acesso == null)
            throw new RecursoNaoEncontradoException("Acesso nao encontrado.");

        _repository.Remove(acesso);

        await _repository.SaveChangesAsync();
    }

    private static void ValidateCommonData(string nome, decimal preco)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new RegraNegocioException("O nome do acesso e obrigatorio.");

        if (preco < 0)
            throw new RegraNegocioException("O preco do acesso nao pode ser negativo.");
    }

    // private static string GetDescricaoTipo(TipoAcesso tipo)
    // {
    //     return tipo switch
    //     {
    //         TipoAcesso.BilheteSessao => "Bilhete válido para uma sessão específica.",

    //         TipoAcesso.PasseDiario =>
    //             "Passe que dá acesso às sessões de um festival durante um dia.",

    //         TipoAcesso.PasseCompleto => "Passe que dá acesso a todas as sessões de um festival.",

    //         _ => "Tipo de acesso desconhecido.",
    //     };
    // }
}
