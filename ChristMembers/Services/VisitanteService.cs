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

    public async Task AlterarStatus(Visitante visitante, StatusVisitante status)
    {
        StatusVisitante anterior = visitante.Status;

        visitante.Status = status;

        await Salvar(visitante, () => visitante.Status = anterior);

        AtualizarNaLista(visitante);
    }

    public async Task SalvarAnotacao(Visitante visitante, string anotacao)
    {
        string anterior = visitante.Anotacao;

        visitante.Anotacao = anotacao.Trim();

        await Salvar(visitante, () => visitante.Anotacao = anterior);

        AtualizarNaLista(visitante);
    }

    public async Task Inativar(Visitante visitante)
    {
        visitante.Ativo = false;

        await Salvar(visitante, () => visitante.Ativo = true);

        Visitantes.Remove(visitante);
    }

    private async Task Salvar(Visitante visitante, Action reverter)
    {
        var cliente = await _supabase.ObterAsync();

        try
        {
            var resposta = await cliente.From<Visitante>().Update(visitante);

            // A data do contato é preenchida pelo banco.
            Visitante? atualizado = resposta.Models.FirstOrDefault();

            if (atualizado != null)
                visitante.DataContato = atualizado.DataContato;
        }
        catch
        {
            reverter();
            throw;
        }
    }

    private void AtualizarNaLista(Visitante visitante)
    {
        int indice = Visitantes.IndexOf(visitante);

        if (indice >= 0)
            Visitantes[indice] = visitante;
    }
}