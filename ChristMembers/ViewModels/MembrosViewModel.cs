using ChristMembers.Models;
using ChristMembers.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Globalization;
using ChristMembers.Helpers;

namespace ChristMembers.ViewModels;

public class MembrosViewModel : INotifyPropertyChanged
{

    private string _nome = string.Empty;
    private string _telefone = string.Empty;
    private string _cpf = string.Empty;
    private string _email = string.Empty;
    private string _dataNascimentoTexto = string.Empty;
    private string _cep = string.Empty;
    private string _endereço = string.Empty;
    private string _numero = string.Empty;
    private string _complemento = string.Empty;
    private string _bairro = string.Empty;
    private string _cidade = string.Empty;
    private string _estado = string.Empty;
    private string _dataEntradaTexto = string.Empty;
    private bool? _foiBatizado = null;
    private string _observacao = string.Empty;
    private string _mensagemErro = string.Empty;
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
    public string CPF
    {
        get => _cpf;
        set => SetProperty(ref _cpf, value);
    }
    public string DataNascimentoTexto
    {
        get => _dataNascimentoTexto;
        set => SetProperty(ref _dataNascimentoTexto, value);
    }
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }
    public string CEP
    {
        get => _cep;
        set => SetProperty(ref _cep, value);
    }
    public string Endereco
    {
        get => _endereço;
        set => SetProperty(ref _endereço, value);
    }
    public string Numero
    {
        get => _numero;
        set => SetProperty(ref _numero, value);
    }
    public string Complemento
    {
        get => _complemento;
        set => SetProperty(ref _complemento, value);
    }
    public string Bairro
    {
        get => _bairro;
        set => SetProperty(ref _bairro, value);
    }
    public string Cidade
    {
        get => _cidade;
        set => SetProperty(ref _cidade, value);
    }
    public string Estado
    {
        get => _estado;
        set => SetProperty(ref _estado, value);
    }
    public string DataEntradaTexto
    {
        get => _dataEntradaTexto;
        set => SetProperty(ref _dataEntradaTexto, value);
    }
    public bool? FoiBatizado
    {
        get => _foiBatizado;

        set
        {
            if (_foiBatizado == value)
                return;

            _foiBatizado = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(BatizadoSim));
            OnPropertyChanged(nameof(BatizadoNao));
        }
    }

    public bool BatizadoSim
    {
        get => FoiBatizado == true;

        set
        {
            if (value)
                FoiBatizado = true;
        }
    }

    public bool BatizadoNao
    {
        get => FoiBatizado == false;

        set
        {
            if (value)
                FoiBatizado = false;
        }
    }

    public string Observacao
    {
        get => _observacao;
        set => SetProperty(ref _observacao, value);
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

    public ObservableCollection<Membro> Membros => _membroService.Membros;

    public ICommand CadastrarCommand { get; }

    public MembrosViewModel(MembroService membroService)
    {
        _membroService = membroService;

        CadastrarCommand = new Command(async () => await Cadastrar());
    }

    private async Task Cadastrar()
    {
        List<string> erros = new();

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("Informe o nome.");

        if (string.IsNullOrWhiteSpace(Telefone))
            erros.Add("Informe o telefone.");

        if (!string.IsNullOrWhiteSpace(CPF) && !CpfValidador.EhValido(CPF))
            erros.Add("CPF inválido.");

        if (!TentarConverterData(DataNascimentoTexto, out DateTime dataNascimento))
            erros.Add("Data de nascimento inválida. Use dd/mm/aaaa.");

        DateTime? dataEntrada = null;

        if (!string.IsNullOrEmpty(DataEntradaTexto) &&
            TentarConverterData(DataEntradaTexto, out DateTime dataConvertida))
        {
            dataEntrada = dataConvertida;
        }

        if (erros.Count > 0)
        {
            MensagemErro = string.Join(Environment.NewLine, erros);
            return;
        }

        MensagemErro = string.Empty;

        Membro membro = new Membro
        {
            Nome = Nome,
            Telefone = Telefone,
            FoiBatizado = FoiBatizado,
            CPF = CPF,
            DataNascimento = dataNascimento,
            Email = Email,
            CEP = CEP,
            Endereco = Endereco,
            Numero = Numero,
            Complemento = Complemento,
            Bairro = Bairro,
            Cidade = Cidade,
            Estado = Estado,
            DataEntrada = dataEntrada,
            Observacao = Observacao
        };

        await _membroService.Cadastrar(membro);

        string nomeCadastrado = membro.Nome.Trim();

        LimparFormulario();

        await Shell.Current.DisplayAlert("Sucesso", $"Membro {nomeCadastrado} cadastrado com sucesso!", "OK");
    }

    private void LimparFormulario()
    {
        Nome = string.Empty;
        Telefone = string.Empty;
        CPF = string.Empty;
        Email = string.Empty;
        CEP = string.Empty;
        Endereco = string.Empty;
        Numero = string.Empty;
        Complemento = string.Empty;
        Bairro = string.Empty;
        Cidade = string.Empty;
        Estado = string.Empty;
        Observacao = string.Empty;
        DataNascimentoTexto = string.Empty;
        DataEntradaTexto = string.Empty;
        FoiBatizado = null;
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

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? propriedade = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor))
            return false;

        campo = valor;
        OnPropertyChanged(propriedade);
        return true;
    }
}