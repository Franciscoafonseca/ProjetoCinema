using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services.AcessosFolder;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoService : IAcessoService
{
    private readonly IAcessoRepository _repository;
    private readonly IValidacaoAcessoStrategyFactory _fabricaValidacao;

    public AcessoService(
        IAcessoRepository repository,
        IValidacaoAcessoStrategyFactory fabricaValidacao
    )
    {
        _repository = repository;
        _fabricaValidacao = fabricaValidacao;
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

        var acesso = AcessoMapper.MapFromCreateDTO(dto);

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
            throw new KeyNotFoundException("Acesso não encontrado.");

        AcessoMapper.MapToExistingAcesso(dto, acesso);

        await _repository.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var acesso = await _repository.ObterPorIdAsync(id);

        if (acesso == null)
            throw new KeyNotFoundException("Acesso não encontrado.");

        _repository.Remove(acesso);

        await _repository.SaveChangesAsync();
    }

    private static void ValidateCommonData(string nome, decimal preco)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do acesso é obrigatório.");

        if (preco < 0)
            throw new ArgumentException("O preço do acesso não pode ser negativo.");
    }

    // private static string GetDescricaoTipo(TipoAcesso tipo)
    // {
    //     return tipo switch
    //     {
    //         TipoAcesso.BilheteSessao => "Bilhete válido para uma sessão específica.",

    //         TipoAcesso.PasseDiario =>
    //             "Passe que dá acesso às sessões de um festival durante um dia.",

    //         TipoAcesso.PasseCompleto => "Passe que dá acesso a todas as sessões de um festival.",

    //         TipoAcesso.AluguerDigital =>
    //             "Aluguer digital de um filme durante uma janela temporal, por exemplo 48 horas.",

    //         _ => "Tipo de acesso desconhecido.",
    //     };
    // }
}
