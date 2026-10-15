using System.Collections.ObjectModel;
using System.Windows.Input;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ListaMembrosViewModel
{
    private readonly MembroService _membroService;
    private readonly AuthService _auth;

    public ObservableCollection<Membro> Membros => _membroService.Membros;

    public ICommand ExcluirCommand { get; }

    public ListaMembrosViewModel(MembroService membroService, AuthService auth)
    {
        _membroService = membroService;
        _auth = auth;

        ExcluirCommand = new Command<Membro>(async membro => await Excluir(membro));
    }

    public async Task Inicializar()
    {
        await _membroService.Inicializar();
    }

    private async Task Excluir(Membro? membro)
    {
        if (membro == null || _auth.UsuarioLogado?.Cargo != AuthService.CargoAdmin)
            return;

        bool confirmou = await Shell.Current.DisplayAlert(
            "Excluir membro",
            $"Deseja excluir \"{membro.Nome}\"? Todos os dados desse cadastro serão apagados.",
            "Excluir",
            "Cancelar");

        if (!confirmou)
            return;

        await _membroService.Excluir(membro);
    }
}