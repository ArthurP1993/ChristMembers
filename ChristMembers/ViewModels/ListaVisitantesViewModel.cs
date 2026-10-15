using System.Collections.ObjectModel;
using System.Windows.Input;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ListaVisitantesViewModel
{
    private readonly VisitanteService _visitanteService;
    private readonly AuthService _auth;

    public ObservableCollection<Visitante> Visitantes => _visitanteService.Visitantes;

    public ICommand ExcluirCommand { get; }

    public ListaVisitantesViewModel(VisitanteService visitanteService, AuthService auth)
    {
        _visitanteService = visitanteService;
        _auth = auth;

        ExcluirCommand = new Command<Visitante>(async visitante => await Excluir(visitante));
    }

    public async Task Inicializar()
    {
        await _visitanteService.Inicializar();
    }

    private async Task Excluir(Visitante? visitante)
    {
        if (visitante == null || _auth.UsuarioLogado?.Cargo != AuthService.CargoAdmin)
            return;

        bool confirmou = await Shell.Current.DisplayAlert(
            "Excluir visitante",
            $"Deseja excluir \"{visitante.Nome}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmou)
            return;

        await _visitanteService.Inativar(visitante);
    }
}