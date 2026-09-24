using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class MembrosViewModel : INotifyPropertyChanged
{
    private string _nome = string.Empty;
    private string _telefone = string.Empty;

    private readonly MembroService _membroService;

    public string Nome
    {
        get => _nome;

        set
        {
            if (_nome == value)
                return;

            _nome = value;

            OnPropertyChanged();
        }
    }

    public string Telefone
    {
        get => _telefone;

        set
        {
            if (_telefone == value)
                return;

            _telefone = value;

            OnPropertyChanged();
        }
    }

    public ObservableCollection<Membro> Membros => _membroService.Membros;

    public ICommand CadastrarCommand { get; }

    public MembrosViewModel(MembroService membroService)
    {
        _membroService = membroService;

        CadastrarCommand = new Command(async () => await Cadastrar());
    }

    private async Task Cadastrar()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            return;

        if (string.IsNullOrWhiteSpace(Telefone))
            return;

        Membro membro = new Membro
        {
            Nome = Nome,
            Telefone = Telefone
        };

        await _membroService.Cadastrar(membro);

        Nome = string.Empty;
        Telefone = string.Empty;
    }

    public async Task Inicializar()
    {
        await _membroService.Inicializar();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propriedade = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propriedade));
    }
}