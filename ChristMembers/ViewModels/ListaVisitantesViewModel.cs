using System.Collections.ObjectModel;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ListaVisitantesViewModel
{
    private readonly VisitanteService _visitanteService;

    public ObservableCollection<Visitante> Visitantes => _visitanteService.Visitantes;

    public ListaVisitantesViewModel(VisitanteService visitanteService)
    {
        _visitanteService = visitanteService;
    }

    public async Task Inicializar()
    {
        await _visitanteService.Inicializar();
    }
}