using ChristMembers.Models;
using Supabase.Postgrest;

namespace ChristMembers.Services;

public class ConfirmacaoService
{
    private readonly SupabaseServico _supabase;

    public ConfirmacaoService(SupabaseServico supabase)
    {
        _supabase = supabase;
    }

    // Qualquer pessoa pode enviar, mas só o administrador lê a lista.
    public async Task EnviarAsync(
        long idNovidade,
        string nome,
        string telefone,
        int pessoas,
        string? idAuth)
    {
        var cliente = await _supabase.ObterAsync();

        await cliente.From<ConfirmacaoPresenca>().Insert(
            new ConfirmacaoPresenca
            {
                IdNovidade = idNovidade,
                IdAuth = idAuth,
                Nome = nome.Trim(),
                Telefone = telefone,
                Pessoas = pessoas
            },
            new QueryOptions { Returning = QueryOptions.ReturnType.Minimal });
    }

    // Quem está logado consegue ver a própria confirmação.
    public async Task<ConfirmacaoPresenca?> ObterMinhaAsync(long idNovidade, string idAuth)
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente
            .From<ConfirmacaoPresenca>()
            .Where(c => c.IdNovidade == idNovidade)
            .Where(c => c.IdAuth == idAuth)
            .Get();

        return resposta.Models.FirstOrDefault();
    }

    public async Task CancelarAsync(ConfirmacaoPresenca confirmacao)
    {
        var cliente = await _supabase.ObterAsync();

        await cliente.From<ConfirmacaoPresenca>().Delete(confirmacao);
    }

    // Só o administrador.
    public async Task<List<ConfirmacaoPresenca>> ListarDaNovidadeAsync(long idNovidade)
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente
            .From<ConfirmacaoPresenca>()
            .Where(c => c.IdNovidade == idNovidade)
            .Get();

        return resposta.Models.OrderBy(c => c.Nome).ToList();
    }
}