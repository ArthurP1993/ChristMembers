using System.Collections.ObjectModel;
using ChristMembers.Models;
using Supabase.Postgrest;

namespace ChristMembers.Services;

public class VisitanteService
{
    private readonly SupabaseServico _supabase;

    public ObservableCollection<Visitante> Visitantes { get; } = new();

    public VisitanteService(SupabaseServico supabase)
    {
        _supabase = supabase;
    }

    // Só o admin consegue ler (as regras do banco garantem isso).
    public async Task Inicializar()
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente
            .From<Visitante>()
            .Where(v => v.Ativo == true)
            .Get();

        Visitantes.Clear();

        foreach (Visitante visitante in resposta.Models.OrderByDescending(v => v.Id))
        {
            Visitantes.Add(visitante);
        }
    }

    // Qualquer pessoa pode enviar, mas não consegue ler de volta.
    public async Task Cadastrar(Visitante visitante)
    {
        var cliente = await _supabase.ObterAsync();

        await cliente
            .From<Visitante>()
            .Insert(visitante, new QueryOptions { Returning = QueryOptions.ReturnType.Minimal });
    }

    public async Task Inativar(Visitante visitante)
    {
        var cliente = await _supabase.ObterAsync();

        visitante.Ativo = false;

        try
        {
            await cliente.From<Visitante>().Update(visitante);
        }
        catch
        {
            visitante.Ativo = true;
            throw;
        }

        Visitantes.Remove(visitante);
    }
}