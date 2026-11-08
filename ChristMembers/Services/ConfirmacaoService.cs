using ChristMembers.Models;
using Newtonsoft.Json;

namespace ChristMembers.Services;

public class RetornoConfirmacao
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;
}

public class ConfirmacaoService
{
    private readonly SupabaseServico _supabase;

    public ConfirmacaoService(SupabaseServico supabase)
    {
        _supabase = supabase;
    }

    // Qualquer pessoa pode confirmar. Devolve o código secreto da confirmação.
    public async Task<RetornoConfirmacao> ConfirmarAsync(
        long idNovidade,
        string nome,
        string telefone)
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.Rpc(
            "confirmar_presenca",
            new Dictionary<string, object>
            {
                { "p_id_novidade", idNovidade },
                { "p_nome", nome.Trim() },
                { "p_telefone", telefone }
            });

        RetornoConfirmacao? retorno =
            JsonConvert.DeserializeObject<RetornoConfirmacao>(resposta.Content ?? string.Empty);

        if (retorno == null || retorno.Id == 0 || string.IsNullOrEmpty(retorno.Token))
            throw new InvalidOperationException("Resposta inesperada ao confirmar a presença.");

        return retorno;
    }

    public async Task<int> AdicionarAcompanhantesAsync(
        long idConfirmacao,
        string token,
        IEnumerable<string> nomes)
    {
        var cliente = await _supabase.ObterAsync();

        // Sem código (membro logado), vai um código vazio: o banco confere pela conta.
        string codigo = string.IsNullOrWhiteSpace(token) ? Guid.Empty.ToString() : token;

        var resposta = await cliente.Rpc(
            "adicionar_acompanhantes",
            new Dictionary<string, object>
            {
                { "p_id_confirmacao", idConfirmacao },
                { "p_token", codigo },
                { "p_nomes", nomes.ToList() }
            });

        return int.TryParse(resposta.Content?.Trim(), out int total) ? total : 0;
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

    // Nomes dos acompanhantes de uma confirmação (o dono e o administrador leem).
    public async Task<List<string>> ListarAcompanhantesAsync(long idConfirmacao)
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente
            .From<ConfirmacaoAcompanhante>()
            .Where(a => a.IdConfirmacao == idConfirmacao)
            .Get();

        return resposta.Models
            .OrderBy(a => a.Id)
            .Select(a => a.Nome)
            .ToList();
    }

    // Cancelar a presença apaga também os acompanhantes (o banco faz isso).
    public async Task CancelarAsync(ConfirmacaoPresenca confirmacao)
    {
        var cliente = await _supabase.ObterAsync();

        await cliente.From<ConfirmacaoPresenca>().Delete(confirmacao);
    }

    // ---------- Só o administrador ----------

    public async Task<List<ConfirmacaoPresenca>> ListarDaNovidadeAsync(long idNovidade)
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente
            .From<ConfirmacaoPresenca>()
            .Where(c => c.IdNovidade == idNovidade)
            .Get();

        return resposta.Models.OrderBy(c => c.Nome).ToList();
    }

    public async Task<List<ConfirmacaoPresenca>> ListarTodasAsync()
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<ConfirmacaoPresenca>().Get();

        return resposta.Models;
    }

    public async Task<List<ConfirmacaoAcompanhante>> ListarTodosAcompanhantesAsync()
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<ConfirmacaoAcompanhante>().Get();

        return resposta.Models;
    }
}