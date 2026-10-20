using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ListaVisitantesViewModel : INotifyPropertyChanged
{
    private readonly VisitanteService _visitanteService;
    private readonly AuthService _auth;

    private string _textoBusca = string.Empty;
    private string _resumo = string.Empty;

    public ObservableCollection<Visitante> Visitantes { get; } = new();

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

    public ListaVisitantesViewModel(VisitanteService visitanteService, AuthService auth)
    {
        _visitanteService = visitanteService;
        _auth = auth;

        ExcluirCommand = new Command<Visitante>(async visitante => await Excluir(visitante));
    }

    public async Task Inicializar()
    {
        try
        {
            await _visitanteService.Inicializar();
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert(
                "Sem conexão",
                "Não foi possível carregar os visitantes. Verifique a internet.",
                "OK");
        }

        AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        Visitantes.Clear();

        foreach (Visitante visitante in _visitanteService.Visitantes
                     .Where(v => BuscaHelper.Combina(TextoBusca, v.Nome, v.Telefone)))
        {
            Visitantes.Add(visitante);
        }

        int total = _visitanteService.Visitantes.Count;

        Resumo = string.IsNullOrWhiteSpace(TextoBusca)
            ? $"Total: {total}"
            : $"Exibindo {Visitantes.Count} de {total}";
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

        try
        {
            await _visitanteService.Inativar(visitante);
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                "Não foi possível excluir. Verifique a internet.",
                "OK");
        }

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