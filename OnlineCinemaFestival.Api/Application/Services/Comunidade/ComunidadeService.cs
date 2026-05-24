using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Mapping;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ComunidadeService : IComunidadeService
{
    private readonly IComunidadeRepository _comunidadeRepository;
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly IImagemUploadService _imagemUploadService;

    public ComunidadeService(
        IComunidadeRepository comunidadeRepository,
        IUtilizadorRepository utilizadorRepository,
        IImagemUploadService imagemUploadService
    )
    {
        _comunidadeRepository = comunidadeRepository;
        _utilizadorRepository = utilizadorRepository;
        _imagemUploadService = imagemUploadService;
    }

    public async Task<IEnumerable<ComunidadeReadDTO>> ObterTodasComunidadesAsync(
        int utilizadorIdPedido
    )
    {
        var comunidades = await _comunidadeRepository.FindComunidadesAsync(c =>
            c.IsPublic && !c.Members.Any(m => m.UtilizadorId == utilizadorIdPedido)
        );

        return comunidades.Select(c => ComunidadeMapper.ToReadDTO(c, utilizadorIdPedido));
    }

    public async Task<IEnumerable<ComunidadeReadDTO>> ObterMinhasComunidadesAsync(int utilizadorId)
    {
        var comunidades = await _comunidadeRepository.FindComunidadesAsync(c =>
            c.Members.Any(m => m.UtilizadorId == utilizadorId)
        );

        return comunidades.Select(c => ComunidadeMapper.ToReadDTO(c, utilizadorId));
    }

    public async Task<ComunidadeReadDTO?> ObterComunidadePorPublicIdAsync(
        Guid publicId,
        int utilizadorIdPedido
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(publicId);

        if (comunidade == null)
            return null;

        var acessoProibido =
            !comunidade.IsPublic
            && !await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizadorIdPedido);

        if (acessoProibido)
            throw new UnauthorizedAccessException("Acesso negado a comunidade privada.");

        return ComunidadeMapper.ToReadDTO(comunidade, utilizadorIdPedido);
    }

    public async Task<ComunidadeReadDTO> CriarComunidadeAsync(
        ComunidadeCreateDTO dto,
        int criadorUtilizadorId
    )
    {
        var criador = await _utilizadorRepository.ObterPorIdAsync(criadorUtilizadorId);

        if (criador == null)
            throw new KeyNotFoundException("Criador nao encontrado.");

        var comunidade = ComunidadeMapper.ToEntity(dto, criadorUtilizadorId);

        comunidade.CodigoConvite = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        comunidade.Members.Add(
            new ComunidadeMembro
            {
                UtilizadorId = criadorUtilizadorId,
                Role = PapelMembroComunidade.Proprietario,
                JoinedAt = DateTime.UtcNow,
            }
        );

        var comunidadeCriada = await _comunidadeRepository.AddComunidadeAsync(comunidade);

        comunidadeCriada.CreatedByUser = criador;

        return ComunidadeMapper.ToReadDTO(comunidadeCriada, criadorUtilizadorId);
    }

    public async Task<ComunidadeReadDTO?> ObterComunidadePorConviteAsync(
        string codigoConvite,
        int utilizadorIdPedido
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByConviteAsync(codigoConvite);

        if (comunidade == null)
            return null;

        return ComunidadeMapper.ToReadDTO(comunidade, utilizadorIdPedido);
    }

    public async Task AderirComunidadeAsync(Guid comunidadePublicId, int utilizadorId)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(
            comunidadePublicId
        );

        var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);

        await ValidarRegrasDeAdesaoAsync(comunidade, utilizador, entradaPorConvite: false);

        var novoMembro = new ComunidadeMembro
        {
            ComunidadeId = comunidade!.Id,
            UtilizadorId = utilizadorId,
            Role = PapelMembroComunidade.Membro,
            JoinedAt = DateTime.UtcNow,
        };

        await _comunidadeRepository.AdicionarMembroAsync(novoMembro);
    }

    public async Task AderirComunidadePorConviteAsync(string codigoConvite, int utilizadorId)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByConviteAsync(codigoConvite);

        var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);

        await ValidarRegrasDeAdesaoAsync(comunidade, utilizador, entradaPorConvite: true);

        var novoMembro = new ComunidadeMembro
        {
            ComunidadeId = comunidade!.Id,
            UtilizadorId = utilizadorId,
            Role = PapelMembroComunidade.Membro,
            JoinedAt = DateTime.UtcNow,
        };

        await _comunidadeRepository.AdicionarMembroAsync(novoMembro);
    }

    public async Task ApagarComunidadeAsync(Guid comunidadePublicId, int utilizadorId)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(
            comunidadePublicId
        );

        if (comunidade == null)
            throw new Exception("Comunidade não encontrada.");

        var isProprietario = await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizadorId)
            && comunidade.Members.First(m => m.UtilizadorId == utilizadorId).Role == PapelMembroComunidade.Proprietario;

        if (!isProprietario)
            throw new UnauthorizedAccessException("Apenas o proprietário pode apagar a comunidade.");

        await _comunidadeRepository.ApagarComunidadeAsync(comunidade);
    }

    public async Task SairComunidadeAsync(Guid comunidadePublicId, int utilizadorId)
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(
            comunidadePublicId
        );

        if (comunidade == null)
            throw new Exception("Comunidade não encontrada.");

        var membro = comunidade.Members.FirstOrDefault(m => m.UtilizadorId == utilizadorId);

        if (membro == null)
            throw new Exception("Utilizador não é membro desta comunidade.");

        if (membro.Role == PapelMembroComunidade.Proprietario)
        {
            var proximoLider = comunidade.Members
                .Where(m => m.UtilizadorId != utilizadorId)
                .OrderBy(m => m.JoinedAt)
                .FirstOrDefault();

            if (proximoLider != null)
            {
                proximoLider.Role = PapelMembroComunidade.Proprietario;
            }
            else
            {
                await _comunidadeRepository.ApagarComunidadeAsync(comunidade);
                return;
            }
        }
        await _comunidadeRepository.RemoverMembroAsync(membro);
    }

    public async Task<ComunidadeReadDTO> EnviarImagemAsync(
        Guid comunidadePublicId,
        int utilizadorId,
        IFormFile ficheiro
    )
    {
        var comunidade = await _comunidadeRepository.GetComunidadeByPublicIdAsync(
            comunidadePublicId
        );

        if (comunidade == null)
            throw new KeyNotFoundException("Comunidade nao encontrada.");

        var isProprietario = await _comunidadeRepository.IsProprietarioAsync(
            comunidade.Id,
            utilizadorId
        );

        if (!isProprietario)
            throw new UnauthorizedAccessException("Apenas o proprietario pode alterar a imagem.");

        comunidade.ImageUrl = await _imagemUploadService.GuardarAsync(ficheiro, "comunidades");
        comunidade.UpdatedAt = DateTime.UtcNow;

        await _comunidadeRepository.SaveChangesAsync();

        return ComunidadeMapper.ToReadDTO(comunidade, utilizadorId);
    }

    private async Task ValidarRegrasDeAdesaoAsync(
        Comunidade? comunidade,
        Utilizador? utilizador,
        bool entradaPorConvite
    )
    {
        if (utilizador == null)
            throw new KeyNotFoundException("Utilizador nao encontrado.");

        if (comunidade == null)
            throw new KeyNotFoundException("Comunidade nao encontrada.");

        var jaEMembro = await _comunidadeRepository.IsMembroAsync(comunidade.Id, utilizador.Id);

        if (jaEMembro)
            throw new InvalidOperationException("Ja es membro desta comunidade.");

        if (!entradaPorConvite && !comunidade.IsPublic)
            throw new UnauthorizedAccessException("Esta comunidade e privada. Precisas de convite.");
    }
}
