using ChristMembers.Models;

namespace ChristMembers.Services;

public class ListasService
{
    private readonly SupabaseServico _supabase;

    private List<Consagracao>? _consagracoes;
    private List<EstadoCivil>? _estadosCivis;
    private List<Genero>? _generos;
    private List<Deficiencia>? _deficiencias;

    public ListasService(SupabaseServico supabase)
    {
        _supabase = supabase;
    }

    // Listas sem dados pessoais: ficam guardadas na memória.
    public async Task<List<Consagracao>> ObterConsagracoesAsync()
    {
        if (_consagracoes != null && _consagracoes.Count > 0)
            return _consagracoes;

        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<Consagracao>().Get();

        _consagracoes = resposta.Models.OrderBy(c => c.Id).ToList();

        return _consagracoes;
    }

    public async Task<List<EstadoCivil>> ObterEstadosCivisAsync()
    {
        if (_estadosCivis != null && _estadosCivis.Count > 0)
            return _estadosCivis;

        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<EstadoCivil>().Get();

        _estadosCivis = resposta.Models.OrderBy(e => e.Id).ToList();

        return _estadosCivis;
    }

    public async Task<List<Genero>> ObterGenerosAsync()
    {
        if (_generos != null && _generos.Count > 0)
            return _generos;

        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<Genero>().Get();

        _generos = resposta.Models.OrderBy(g => g.Id).ToList();

        return _generos;
    }

    public async Task<List<Deficiencia>> ObterDeficienciasAsync()
    {
        if (_deficiencias != null && _deficiencias.Count > 0)
            return _deficiencias;

        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<Deficiencia>().Get();

        _deficiencias = resposta.Models.OrderBy(d => d.Id).ToList();

        return _deficiencias;
    }

    // Não fica guardada: quem não é admin só enxerga o próprio setor.
    public async Task<List<Setor>> ObterSetoresAsync()
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<Setor>().Get();

        return resposta.Models.OrderBy(s => s.Descricao).ToList();
    }
}