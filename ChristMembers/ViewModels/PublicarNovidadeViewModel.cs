using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class PublicarNovidadeViewModel : INotifyPropertyChanged
{
    private readonly NovidadeService _novidadeService;
    private readonly AuthService _auth;

    private string _titulo = string.Empty;
    private string _descricao = string.Empty;
    private string _dataEventoTexto = string.Empty;
    private string _mensagemErro = string.Empty;
    private FileResult? _foto;
    private bool _publicando;

    public string Titulo
    {
        get => _titulo;
        set => SetProperty(ref _titulo, value ?? string.Empty);
    }

    public string Descricao
    {
        get => _descricao;
        set => SetProperty(ref _descricao, value ?? string.Empty);
    }

    public string DataEventoTexto
    {
        get => _dataEventoTexto;
        set => SetProperty(ref _dataEventoTexto, value ?? string.Empty);
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

    private FileResult? Foto
    {
        get => _foto;

        set
        {
            _foto = value;

            OnPropertyChanged(nameof(PreviaFoto));
            OnPropertyChanged(nameof(TemFoto));
            OnPropertyChanged(nameof(TextoBotaoFoto));
        }
    }

    public ImageSource? PreviaFoto =>
        _foto == null ? null : ImageSource.FromFile(_foto.FullPath);

    public bool TemFoto => _foto != null;

    public string TextoBotaoFoto => TemFoto ? "Trocar foto" : "Escolher foto";

    public ICommand EscolherFotoCommand { get; }
    public ICommand RemoverFotoCommand { get; }
    public ICommand PublicarCommand { get; }

    public PublicarNovidadeViewModel(NovidadeService novidadeService, AuthService auth)
    {
        _novidadeService = novidadeService;
        _auth = auth;

        EscolherFotoCommand = new Command(async () => await EscolherFoto());
        RemoverFotoCommand = new Command(() => Foto = null);
        PublicarCommand = new Command(async () => await Publicar());
    }

    private async Task EscolherFoto()
    {
        try
        {
            FileResult? resultado = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Escolha uma foto",
                FileTypes = FilePickerFileType.Images
            });

            if (resultado != null)
                Foto = resultado;
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível abrir a galeria.";
        }
    }

    private async Task Publicar()
    {
        if (_publicando)
            return;

        if (_auth.UsuarioLogado?.Cargo != AuthService.CargoAdmin)
        {
            MensagemErro = "Apenas o administrador pode publicar novidades.";
            return;
        }

        List<string> erros = new();

        if (string.IsNullOrWhiteSpace(Titulo))
            erros.Add("Informe o título.");

        DateTime? dataEvento = null;

        if (!string.IsNullOrWhiteSpace(DataEventoTexto))
        {
            if (!DateTime.TryParseExact(
                    DataEventoTexto,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime data))
            {
                erros.Add("Data do evento inválida. Use dd/mm/aaaa.");
            }
            else if (data.Date < DateTime.Today)
            {
                erros.Add("A data do evento já passou.");
            }
            else
            {
                dataEvento = data;
            }
        }

        if (erros.Count > 0)
        {
            MensagemErro = string.Join(Environment.NewLine, erros);
            return;
        }

        MensagemErro = string.Empty;
        _publicando = true;

        try
        {
            await _novidadeService.Publicar(Titulo, Descricao, dataEvento, Foto);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível publicar. Tente novamente.";
        }
        finally
        {
            _publicando = false;
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