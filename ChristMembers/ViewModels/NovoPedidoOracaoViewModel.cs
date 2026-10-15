using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class NovoPedidoOracaoViewModel : INotifyPropertyChanged
{
    private const int LimiteTexto = 500;

    private readonly PedidoOracaoService _pedidoService;

    private bool _anonimo;
    private string _nome = string.Empty;
    private string _telefone = string.Empty;
    private string? _categoria;
    private string _texto = string.Empty;
    private bool _permiteMural;
    private string _mensagemErro = string.Empty;
    private bool _enviado;
    private bool _enviando;

    public List<string> Categorias => CategoriasOracao.Todas;

    public bool Anonimo
    {
        get => _anonimo;

        set
        {
            if (!SetProperty(ref _anonimo, value))
                return;

            if (value)
            {
                Nome = string.Empty;
                Telefone = string.Empty;
            }

            OnPropertyChanged(nameof(NaoAnonimo));
        }
    }

    public bool NaoAnonimo => !Anonimo;

    public string Nome
    {
        get => _nome;
        set => SetProperty(ref _nome, value ?? string.Empty);
    }

    public string Telefone
    {
        get => _telefone;
        set => SetProperty(ref _telefone, value ?? string.Empty);
    }

    public string? Categoria
    {
        get => _categoria;
        set => SetProperty(ref _categoria, value);
    }

    public string Texto
    {
        get => _texto;

        set
        {
            if (SetProperty(ref _texto, value ?? string.Empty))
                OnPropertyChanged(nameof(ContadorTexto));
        }
    }

    public string ContadorTexto => $"{Texto.Length}/{LimiteTexto}";

    public bool PermiteMural
    {
        get => _permiteMural;
        set => SetProperty(ref _permiteMural, value);
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

    public bool Enviado
    {
        get => _enviado;

        set
        {
            if (SetProperty(ref _enviado, value))
                OnPropertyChanged(nameof(MostrarFormulario));
        }
    }

    public bool MostrarFormulario => !Enviado;

    public ICommand EnviarCommand { get; }

    public ICommand ConcluirCommand { get; }

    public NovoPedidoOracaoViewModel(PedidoOracaoService pedidoService)
    {
        _pedidoService = pedidoService;

        EnviarCommand = new Command(async () => await Enviar());
        ConcluirCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    private async Task Enviar()
    {
        if (_enviando)
            return;

        List<string> erros = new();

        if (string.IsNullOrWhiteSpace(Categoria))
            erros.Add("Escolha a categoria do pedido.");

        if (string.IsNullOrWhiteSpace(Texto))
            erros.Add("Escreva o seu pedido.");

        if (!Anonimo
            && !string.IsNullOrWhiteSpace(Telefone)
            && CpfValidador.SomenteDigitos(Telefone).Length < 10)
        {
            erros.Add("Telefone inválido. Use (11) 99999-9999 ou deixe em branco.");
        }

        if (erros.Count > 0)
        {
            MensagemErro = string.Join(Environment.NewLine, erros);
            return;
        }

        MensagemErro = string.Empty;
        _enviando = true;

        try
        {
            await _pedidoService.Enviar(
                Nome,
                Anonimo,
                Telefone,
                Categoria!,
                Texto,
                PermiteMural);

            Enviado = true;
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível enviar agora. Tente novamente.";
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