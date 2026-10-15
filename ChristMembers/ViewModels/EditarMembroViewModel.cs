using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class EditarMembroViewModel : INotifyPropertyChanged
{
    private readonly MembroService _membroService;
    private readonly AuthService _auth;

    private Membro? _membro;
    private bool _salvando;
    private bool? _foiBatizado;

    private string _nome = string.Empty;
    private string _telefone = string.Empty;
    private string _cpf = string.Empty;
    private string _dataNascimentoTexto = string.Empty;
    private string _email = string.Empty;
    private string _cep = string.Empty;
    private string _endereco = string.Empty;
    private string _numero = string.Empty;
    private string _complemento = string.Empty;
    private string _bairro = string.Empty;
    private string _cidade = string.Empty;
    private string _estado = string.Empty;
    private string _dataEntradaTexto = string.Empty;
    private string _observacao = string.Empty;
    private string _mensagemErro = string.Empty;

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

    public string CPF
    {
        get => _cpf;
        set => SetProperty(ref _cpf, value ?? string.Empty);
    }

    public string DataNascimentoTexto
    {
        get => _dataNascimentoTexto;
        set => SetProperty(ref _dataNascimentoTexto, value ?? string.Empty);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value ?? string.Empty);
    }

    public string CEP
    {
        get => _cep;
        set => SetProperty(ref _cep, value ?? string.Empty);
    }

    public string Endereco
    {
        get => _endereco;
        set => SetProperty(ref _endereco, value ?? string.Empty);
    }

    public string Numero
    {
        get => _numero;
        set => SetProperty(ref _numero, value ?? string.Empty);
    }

    public string Complemento
    {
        get => _complemento;
        set => SetProperty(ref _complemento, value ?? string.Empty);
    }

    public string Bairro
    {
        get => _bairro;
        set => SetProperty(ref _bairro, value ?? string.Empty);
    }

    public string Cidade
    {
        get => _cidade;
        set => SetProperty(ref _cidade, value ?? string.Empty);
    }

    public string Estado
    {
        get => _estado;
        set => SetProperty(ref _estado, value ?? string.Empty);
    }

    public string DataEntradaTexto
    {
        get => _dataEntradaTexto;
        set => SetProperty(ref _dataEntradaTexto, value ?? string.Empty);
    }

    public string Observacao
    {
        get => _observacao;
        set => SetProperty(ref _observacao, value ?? string.Empty);
    }

    public bool BatizadoSim
    {
        get => _foiBatizado == true;

        set
        {
            if (value)
                DefinirBatismo(true);
        }
    }

    public bool BatizadoNao
    {
        get => _foiBatizado == false;

        set
        {
            if (value)
                DefinirBatismo(false);
        }
    }

    public bool BatizadoNaoInformado
    {
        get => _foiBatizado == null;

        set
        {
            if (value)
                DefinirBatismo(null);
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

    public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

    public ICommand SalvarCommand { get; }

    public ICommand CancelarCommand { get; }

    public EditarMembroViewModel(MembroService membroService, AuthService auth)
    {
        _membroService = membroService;
        _auth = auth;

        SalvarCommand = new Command(async () => await Salvar());
        CancelarCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    public void Carregar(Membro membro)
    {
        _membro = membro;

        _nome = membro.Nome ?? string.Empty;
        _telefone = membro.Telefone ?? string.Empty;
        _cpf = membro.CPF ?? string.Empty;
        _email = membro.Email ?? string.Empty;
        _cep = membro.CEP ?? string.Empty;
        _endereco = membro.Endereco ?? string.Empty;
        _numero = membro.Numero ?? string.Empty;
        _complemento = membro.Complemento ?? string.Empty;
        _bairro = membro.Bairro ?? string.Empty;
        _cidade = membro.Cidade ?? string.Empty;
        _estado = membro.Estado ?? string.Empty;
        _observacao = membro.Observacao ?? string.Empty;
        _foiBatizado = membro.FoiBatizado;

        DateTime? nascimento = membro.DataNascimento;
        DateTime? entrada = membro.DataEntrada;

        _dataNascimentoTexto = FormatarData(nascimento);
        _dataEntradaTexto = FormatarData(entrada);

        _mensagemErro = string.Empty;

        OnPropertyChanged(string.Empty);
    }

    private void DefinirBatismo(bool? valor)
    {
        if (_foiBatizado == valor)
            return;

        _foiBatizado = valor;

        OnPropertyChanged(nameof(BatizadoSim));
        OnPropertyChanged(nameof(BatizadoNao));
        OnPropertyChanged(nameof(BatizadoNaoInformado));
    }

    private async Task Salvar()
    {
        if (_salvando || _membro == null)
            return;

        if (_auth.UsuarioLogado?.Cargo != AuthService.CargoAdmin)
        {
            MensagemErro = "Apenas o administrador pode editar cadastros.";
            return;
        }

        List<string> erros = new();

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("Informe o nome.");

        if (CpfValidador.SomenteDigitos(Telefone).Length < 10)
            erros.Add("Telefone inválido. Use (11) 99999-9999.");

        if (!string.IsNullOrWhiteSpace(CPF) && !CpfValidador.EhValido(CPF))
            erros.Add("CPF inválido.");

        DateTime? nascimento = null;

        if (!string.IsNullOrWhiteSpace(DataNascimentoTexto))
        {
            if (!TentarConverterData(DataNascimentoTexto, out DateTime dataNascimento))
                erros.Add("Data de nascimento inválida. Use dd/mm/aaaa.");
            else if (dataNascimento.Date > DateTime.Today)
                erros.Add("A data de nascimento não pode estar no futuro.");
            else
                nascimento = dataNascimento;
        }

        DateTime? entrada = null;

        if (!string.IsNullOrWhiteSpace(DataEntradaTexto))
        {
            if (!TentarConverterData(DataEntradaTexto, out DateTime dataEntrada))
                erros.Add("Data de entrada inválida. Use dd/mm/aaaa.");
            else
                entrada = dataEntrada;
        }

        if (erros.Count > 0)
        {
            MensagemErro = string.Join(Environment.NewLine, erros);
            return;
        }

        MensagemErro = string.Empty;
        _salvando = true;

        try
        {
            _membro.Nome = Nome.Trim();
            _membro.Telefone = Telefone.Trim();
            _membro.CPF = CPF.Trim();
            _membro.DataNascimento = nascimento ?? default(DateTime);
            _membro.Email = Email.Trim();
            _membro.CEP = CEP.Trim();
            _membro.Endereco = Endereco.Trim();
            _membro.Numero = Numero.Trim();
            _membro.Complemento = Complemento.Trim();
            _membro.Bairro = Bairro.Trim();
            _membro.Cidade = Cidade.Trim();
            _membro.Estado = Estado.Trim();
            _membro.DataEntrada = entrada ?? default(DateTime);
            _membro.FoiBatizado = _foiBatizado;
            _membro.Observacao = Observacao.Trim();

            await _membroService.Atualizar(_membro);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível salvar. Tente novamente.";
        }
        finally
        {
            _salvando = false;
        }
    }

    private static string FormatarData(DateTime? data)
    {
        if (!data.HasValue || data.Value.Year <= 1900)
            return string.Empty;

        return data.Value.ToString("dd/MM/yyyy");
    }

    private static bool TentarConverterData(string texto, out DateTime data)
    {
        return DateTime.TryParseExact(
            texto,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out data);
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