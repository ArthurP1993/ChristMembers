using ChristMembers.Models;

namespace ChristMembers.Services;

public class ListasService
{
    private readonly SupabaseServico _supabase;

    private List<Consagracao>? _consagracoes;

    public ListasService(SupabaseServico supabase)
    {
        _supabase = supabase;
    }

    // Lista pública, então pode ficar guardada na memória.
    public async Task<List<Consagracao>> ObterConsagracoesAsync()
    {
        if (_consagracoes != null && _consagracoes.Count > 0)
            return _consagracoes;

        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<Consagracao>().Get();

        _consagracoes = resposta.Models.OrderBy(c => c.Id).ToList();

        return _consagracoes;
    }

    // Não fica guardada: quem não é admin só enxerga o próprio setor.
    public async Task<List<Setor>> ObterSetoresAsync()
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<Setor>().Get();

        return resposta.Models.OrderBy(s => s.Descricao).ToList();
    }
}