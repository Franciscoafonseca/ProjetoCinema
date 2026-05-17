using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class MensagemChatSessaoRepository : IMensagemChatSessaoRepository
{
    private readonly AppDbContext _context;

    public MensagemChatSessaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Sessao?> ObterSessaoPorIdAsync(int sessaoId)
    {
        return await _context
            .Sessoes.AsNoTracking()
            .Include(s => s.Festival)
            .Include(s => s.FilmesDaSessao)
            .FirstOrDefaultAsync(s => s.Id == sessaoId);
    }

    public async Task AdicionarAsync(MensagemChatSessao mensagem)
    {
        await _context.MensagensChatSessao.AddAsync(mensagem);
    }

    public async Task<IReadOnlyList<MensagemChatSessao>> ListarHistoricoRecenteAsync(
        int sessaoId,
        int quantidade
    )
    {
        return await _context
            .MensagensChatSessao.AsNoTracking()
            .Include(m => m.Utilizador)
            .Where(m => m.SessaoId == sessaoId)
            .OrderByDescending(m => m.EnviadaEm)
            .Take(quantidade)
            .OrderBy(m => m.EnviadaEm)
            .ToListAsync();
    }

    public async Task<MensagemChatSessao?> ObterMensagemPorIdAsync(string mensagemId)
    {
        return await _context.MensagensChatSessao.FirstOrDefaultAsync(m => m.Id == mensagemId);
    }

    public void MarcarMensagemRemovida(MensagemChatSessao mensagem)
    {
        mensagem.Removida = true;
        mensagem.RemovidaPorModeracao = true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
