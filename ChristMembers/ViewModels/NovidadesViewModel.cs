using System.Collections.ObjectModel;
using System.Windows.Input;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class NovidadesViewModel
{
    private readonly NovidadeService _novidadeService;
    private readonly AuthService _auth;

    public ObservableCollection<Novidade> Novidades => _novidadeService.Novidades;

    public bool EhAdmin => _auth.UsuarioLogado?.Cargo == AuthService.CargoAdmin;

    public ICommand ExcluirCommand { get; }

    public NovidadesViewModel(NovidadeService novidadeService, AuthService auth)
    {
        _novidadeService = novidadeService;
        _auth = auth;

        ExcluirCommand = new Command<Novidade>(async novidade => await Excluir(novidade));
    }

    public async Task Inicializar()
    {
        await _novidadeService.Inicializar();
    }

    private async Task Excluir(Novidade? novidade)
    {
        if (novidade == null || !EhAdmin)
            return;

        bool confirmou = await Shell.Current.DisplayAlert(
            "Remover novidade",
            $"Deseja remover \"{novidade.Titulo}\"? A foto também será apagada.",
            "Remover",
            "Cancelar");

        if (!confirmou)
            return;

        await _novidadeService.Excluir(novidade);
    }
}