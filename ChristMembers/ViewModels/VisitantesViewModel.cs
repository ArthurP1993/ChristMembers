using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class VisitantesViewModel : INotifyPropertyChanged
{
    private readonly VisitanteService _visitanteService;
    private int _versaoMensagemSucesso;

    private string _nome = string.Empty;
    private string _telefone = string.Empty;
    private string _mensagemErro = string.Empty;
    private string _mensagemSucesso = string.Empty;
    private bool _enviando;

    public string Nome
    {
        get => _nome;

        set
        {
            if (SetProperty(ref _nome, value ?? string.Empty))
                MensagemSucesso = string.Empty;
        }
    }

    public string Telefone
    {
        get => _telefone;

        set
        {
            if (SetProperty(ref _telefone, value ?? string.Empty))
                MensagemSucesso = string.Empty;
        }
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
    private async Task MostrarSucesso(string mensagem)
    {
        int versao = ++_versaoMensagemSucesso;

        MensagemSucesso = mensagem;

        await Task.Delay(4000);

        // Só esconde se não houve outro cadastro nesse intervalo
        if (versao == _versaoMensagemSucesso)
            MensagemSucesso = string.Empty;
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

    public ICommand CadastrarCommand { get; }

    public VisitantesViewModel(VisitanteService visitanteService)
    {
        _visitanteService = visitanteService;

        CadastrarCommand = new Command(async () => await Cadastrar());
    }

    private async Task Cadastrar()
    {
        MensagemSucesso = string.Empty;

        if (_enviando)
            return;

        List<string> erros = new();

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("Informe o nome.");
        else if (Nome.Trim().Length > 120)
            erros.Add("O nome está muito longo.");

        if (CpfValidador.SomenteDigitos(Telefone).Length < 10)
            erros.Add("Telefone inválido. Use (11) 99999-9999.");

        if (erros.Count > 0)
        {
            MensagemSucesso = string.Empty;
            MensagemErro = string.Join(Environment.NewLine, erros);
            return;
        }

        MensagemErro = string.Empty;
        _enviando = true;

        try
        {
            await _visitanteService.Cadastrar(new Visitante
            {
                Nome = Nome.Trim(),
                Telefone = Telefone.Trim()
            });

            Visitante visitante = new Visitante
            {
                Nome = Nome.Trim(),
                Telefone = Telefone.Trim()
            };

            string nomeCadastrado = visitante.Nome;

            Nome = string.Empty;
            Telefone = string.Empty;

            await Shell.Current.DisplayAlert("Sucesso", "Visitante cadastrado com sucesso!", "OK");
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível enviar. Verifique a internet e tente novamente.";
        }
        finally
        {
            _enviando = false;
        }
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