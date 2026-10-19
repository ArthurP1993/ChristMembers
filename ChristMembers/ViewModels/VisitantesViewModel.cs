using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class VisitantesViewModel : INotifyPropertyChanged
{
    private readonly VisitanteService _visitanteService;

    private string _nome = string.Empty;
    private string _telefone = string.Empty;
    private string _mensagemErro = string.Empty;
    private string _mensagemSucesso = string.Empty;
    private int _versaoMensagemSucesso;

    public string Nome
    {
        get => _nome;
        set => SetProperty(ref _nome, value);
    }

    public string Telefone
    {
        get => _telefone;
        set => SetProperty(ref _telefone, value);
    }

    public string MensagemErro
    {
        get => _mensagemErro;

        set
        {
            if (SetProperty(ref _mensagemErro, value))
                OnPropertyChanged(nameof(TemErro));
        }
    }

    public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

    public string MensagemSucesso
    {
        get => _mensagemSucesso;

        set
        {
            if (SetProperty(ref _mensagemSucesso, value))
                OnPropertyChanged(nameof(TemSucesso));
        }
    }

    public bool TemSucesso => !string.IsNullOrEmpty(MensagemSucesso);

    public ObservableCollection<Visitante> Visitantes => _visitanteService.Visitantes;

    public ICommand CadastrarCommand { get; }

    public VisitantesViewModel(VisitanteService visitanteService)
    {
        _visitanteService = visitanteService;

        CadastrarCommand = new Command(async () => await Cadastrar());
    }

    private async Task Cadastrar()
    {
        MensagemSucesso = string.Empty;

        List<string> erros = new();

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("Informe o nome.");

        if (string.IsNullOrWhiteSpace(Telefone))
            erros.Add("Informe o telefone.");

        if (erros.Count > 0)
        {
            MensagemErro = string.Join(Environment.NewLine, erros);
            return;
        }

        MensagemErro = string.Empty;

        Visitante visitante = new Visitante
        {
            Nome = Nome.Trim(),
            Telefone = Telefone.Trim()
        };

        await _visitanteService.Cadastrar(visitante);

        string nomeCadastrado = visitante.Nome;

        LimparFormulario();

        await Shell.Current.DisplayAlert("Sucesso", "Visitante cadastrado com sucesso!", "OK");
    }

    private async Task MostrarSucesso(string mensagem)
    {
        int versao = ++_versaoMensagemSucesso;

        MensagemSucesso = mensagem;

        await Task.Delay(4000);

        // Só esconde se não houve outro cadastro nesse intervalo
        if (versao == _versaoMensagemSucesso)
            MensagemSucesso = string.Empty;
    }

    private void LimparFormulario()
    {
        Nome = string.Empty;
        Telefone = string.Empty;
    }

    public async Task Inicializar()
    {
        await _visitanteService.Inicializar();
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