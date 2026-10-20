using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ListaMembrosViewModel : INotifyPropertyChanged
{
    private readonly MembroService _membroService;
    private readonly AuthService _auth;

    private string _textoBusca = string.Empty;
    private string _resumo = string.Empty;

    public ObservableCollection<Membro> Membros { get; } = new();

    public string TextoBusca
    {
        get => _textoBusca;

        set
        {
            if (SetProperty(ref _textoBusca, value ?? string.Empty))
                AplicarFiltro();
        }
    }

    public string Resumo
    {
        get => _resumo;
        private set => SetProperty(ref _resumo, value);
    }

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

        AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        Membros.Clear();

        foreach (Membro membro in _membroService.Membros
                     .Where(m => BuscaHelper.Combina(TextoBusca, m.Nome, m.Telefone)))
        {
            Membros.Add(membro);
        }

        int total = _membroService.Membros.Count;

        Resumo = string.IsNullOrWhiteSpace(TextoBusca)
            ? $"Total: {total}"
            : $"Exibindo {Membros.Count} de {total}";
    }

    private async Task Excluir(Membro? membro)
    {
        if (membro == null || _auth.UsuarioLogado?.Cargo != AuthService.CargoAdmin)
            return;

        bool confirmou = await Shell.Current.DisplayAlert(
            "Excluir membro",
            $"Deseja excluir \"{membro.Nome}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmou)
            return;

        await _membroService.Inativar(membro);

        AplicarFiltro();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propriedade = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propriedade));
    }

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? propriedade = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor))
            return false;

        campo = valor;
        OnPropertyChanged(propriedade);
        return true;
    }
}