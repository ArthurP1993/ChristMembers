using System.Collections.ObjectModel;
using ChristMembers.Models;

namespace ChristMembers.Services;

public class MembroService
{
    private readonly SupabaseServico _supabase;
    private readonly ListasService _listas;

    public ObservableCollection<Membro> Membros { get; } = new();

    public MembroService(SupabaseServico supabase, ListasService listas)
    {
        _supabase = supabase;
        _listas = listas;
    }

    // Só o admin consegue ler a lista inteira (as regras do banco garantem isso).
    public async Task Inicializar()
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente
            .From<Membro>()
            .Where(m => m.Ativo == true)
            .Get();

        List<Membro> membros = resposta.Models.OrderBy(m => m.Nome).ToList();

        await PreencherNomesAsync(membros, true);

        Membros.Clear();

        foreach (Membro membro in membros)
        {
            Membros.Add(membro);
        }
    }

    // Cada conta lê apenas o próprio cadastro.
    public async Task<Membro?> ObterPorAuthAsync(string idAuth)
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente
            .From<Membro>()
            .Where(m => m.IdAuth == idAuth)
            .Get();

        Membro? membro = resposta.Models.FirstOrDefault();

        if (membro != null)
            await PreencherNomesAsync(new[] { membro }, false);

        return membro;
    }

    // O cadastro precisa estar ligado à conta de quem está logado.
    public async Task<Membro?> Cadastrar(Membro membro)
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<Membro>().Insert(membro);

        return resposta.Models.FirstOrDefault();
    }

    public async Task Atualizar(Membro membro)
    {
        var cliente = await _supabase.ObterAsync();

        await cliente.From<Membro>().Update(membro);

        await PreencherNomesAsync(new[] { membro }, true);

        int indice = Membros.IndexOf(membro);

        if (indice >= 0)
            Membros[indice] = membro;
    }

    public async Task Inativar(Membro membro)
    {
        var cliente = await _supabase.ObterAsync();

        membro.Ativo = false;

        try
        {
            await cliente.From<Membro>().Update(membro);
        }
        catch
        {
            membro.Ativo = true;
            throw;
        }

        Membros.Remove(membro);
    }

    // Traduz os códigos em nomes. Se as listas não carregarem, os nomes ficam vazios.
    private async Task PreencherNomesAsync(IEnumerable<Membro> membros, bool incluirSetores)
    {
        Dictionary<long, string> consagracoes = new();
        Dictionary<long, string> setores = new();

        try
        {
            foreach (Consagracao c in await _listas.ObterConsagracoesAsync())
                consagracoes[c.Id] = c.Descricao;
        }
        catch (Exception)
        {
        }

        if (incluirSetores)
        {
            try
            {
                foreach (Setor s in await _listas.ObterSetoresAsync())
                    setores[s.Id] = s.Descricao;
            }
            catch (Exception)
            {
            }
        }

        foreach (Membro membro in membros)
        {
            membro.NomeConsagracao =
                membro.IdConsagracao.HasValue
                && consagracoes.TryGetValue(membro.IdConsagracao.Value, out string? nomeConsagracao)
                    ? nomeConsagracao
                    : string.Empty;

            membro.NomeSetor =
                membro.IdSetor.HasValue
                && setores.TryGetValue(membro.IdSetor.Value, out string? nomeSetor)
                    ? nomeSetor
                    : string.Empty;
        }
    }
}