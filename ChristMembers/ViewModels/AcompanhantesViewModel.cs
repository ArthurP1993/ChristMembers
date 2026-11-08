using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ItemNome
{
    public string Nome { get; set; } = string.Empty;
}

public class AcompanhantesViewModel : INotifyPropertyChanged
{
    private const int LimiteTotal = 10;

    private readonly ConfirmacaoService _confirmacoes;

    private long _idConfirmacao;
    private string _token = string.Empty;
    private int _existentes;
    private string _titulo = string.Empty;
    private Action<List<string>>? _aoConcluir;
    private string _mensagemErro = string.Empty;
    private bool _ocupado;

    public ObservableCollection<ItemNome> Nomes { get; } = new();

    public string Titulo
    {
        get => _titulo;
        private set => SetProperty(ref _titulo, value);
    }

    public bool PodeAdicionar => Nomes.Count + _existentes < LimiteTotal;

    public string TextoLimite => _existentes == 0
        ? $"Você pode incluir até {LimiteTotal} acompanhantes."
        : $"Você já tem {_existentes}. Pode incluir mais {Math.Max(0, LimiteTotal - _existentes)}.";

    public string MensagemErro
    {
        get => _mensagemErro;

        set
        {
            if (SetProperty(ref _mensagemErro, value ?? string.Empty))
                OnPropertyChanged(nameof(TemErro));
        }
    }

    public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

    public ICommand AdicionarCommand { get; }
    public ICommand ConfirmarCommand { get; }
    public ICommand PularCommand { get; }

    public AcompanhantesViewModel(ConfirmacaoService confirmacoes)
    {
        _confirmacoes = confirmacoes;

        Nomes.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PodeAdicionar));

        AdicionarCommand = new Command(Adicionar);
        ConfirmarCommand = new Command(async () => await Confirmar());
        PularCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        Nomes.Add(new ItemNome());
    }

    public void Definir(
        long idConfirmacao,
        string token,
        string titulo,
        int existentes,
        Action<List<string>>? aoConcluir)
    {
        _idConfirmacao = idConfirmacao;
        _token = token;
        _existentes = existentes;
        _aoConcluir = aoConcluir;

        Titulo = titulo;

        Nomes.Clear();
        Nomes.Add(new ItemNome());

        MensagemErro = string.Empty;

        OnPropertyChanged(nameof(PodeAdicionar));
        OnPropertyChanged(nameof(TextoLimite));
    }

    public void Remover(ItemNome item)
    {
        // A última linha é trocada por uma vazia, para o campo ficar limpo.
        if (Nomes.Count <= 1)
        {
            Nomes.Clear();
            Nomes.Add(new ItemNome());
            return;
        }

        Nomes.Remove(item);
    }

    private void Adicionar()
    {
        MensagemErro = string.Empty;

        if (PodeAdicionar)
            Nomes.Add(new ItemNome());
    }

    private async Task Confirmar()
    {
        if (_ocupado)
            return;

        List<string> nomes = Nomes
            .Select(n => n.Nome?.Trim() ?? string.Empty)
            .Where(n => n.Length > 0)
            .ToList();

        if (nomes.Count == 0)
        {
            MensagemErro = "Digite pelo menos um nome ou toque em Agora não.";
            return;
        }

        if (nomes.Any(n => n.Length > 120))
        {
            MensagemErro = "Cada nome pode ter no máximo 120 caracteres.";
            return;
        }

        if (nomes.Count + _existentes > LimiteTotal)
        {
            MensagemErro = $"O limite é de {LimiteTotal} acompanhantes por confirmação.";
            return;
        }

        _ocupado = true;
        MensagemErro = string.Empty;

        try
        {
            await _confirmacoes.AdicionarAcompanhantesAsync(_idConfirmacao, _token, nomes);

            _aoConcluir?.Invoke(nomes);

            await Shell.Current.DisplayAlert(
                "Pronto",
                nomes.Count == 1
                    ? "1 acompanhante confirmado."
                    : $"{nomes.Count} acompanhantes confirmados.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            MensagemErro = TraduzirErro(ex);
        }
        finally
        {
            _ocupado = false;
        }
    }

    private static string TraduzirErro(Exception ex)
    {
        string texto = ex.Message;

        if (texto.Contains("Limite", StringComparison.OrdinalIgnoreCase))
            return $"O limite é de {LimiteTotal} acompanhantes por confirmação.";

        if (texto.Contains("42501") || texto.Contains("permiss", StringComparison.OrdinalIgnoreCase)
            || texto.Contains("encerradas", StringComparison.OrdinalIgnoreCase))
        {
            return "Não foi possível confirmar. O evento pode ter sido encerrado, ou esta confirmação não é deste aparelho.";
        }

        return "Não foi possível confirmar. Verifique a internet e tente novamente.";
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