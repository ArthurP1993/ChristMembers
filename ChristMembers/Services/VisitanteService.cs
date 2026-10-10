using System.Collections.ObjectModel;
using ChristMembers.Data;
using ChristMembers.Models;

namespace ChristMembers.Services;

public class VisitanteService
{
    private readonly BancoDados _bancoDados;

    public ObservableCollection<Visitante> Visitantes { get; } =
        new ObservableCollection<Visitante>();

    public VisitanteService(BancoDados bancoDados)
    {
        _bancoDados = bancoDados;
    }

    public async Task Inicializar()
    {
        List<Visitante> visitantes = await _bancoDados.ListarVisitantes();

        Visitantes.Clear();

        foreach (Visitante visitante in visitantes)
        {
            Visitantes.Add(visitante);
        }
    }

    public async Task Cadastrar(Visitante visitante)
    {
        visitante.DataVisita = DateTime.Today;

        await _bancoDados.SalvarVisitante(visitante);

        Visitantes.Add(visitante);
    }

    public async Task Excluir(Visitante visitante)
    {
        await _bancoDados.ExcluirVisitante(visitante);

        Visitantes.Remove(visitante);
    }
}