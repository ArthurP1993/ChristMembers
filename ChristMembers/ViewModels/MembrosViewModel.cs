using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class MembrosViewModel : INotifyPropertyChanged
{
    private readonly MembroService _membroService;
    private readonly AuthService _auth;
    private bool _consentimento;

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
    private bool? _foiBatizado;
    private string _mensagemErro = string.Empty;
    private string _mensagemSucesso = string.Empty;
    private bool _enviando;
    public bool Consentimento
    {
        get => _consentimento;
        set => SetProperty(ref _consentimento, value);
    }

    public string Nome
    {
        get => _nome;
        set { if (SetProperty(ref _nome, value ?? string.Empty)) LimparSucesso(); }
    }

    public string Telefone
    {
        get => _telefone;
        set { if (SetProperty(ref _telefone, value ?? string.Empty)) LimparSucesso(); }
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
        set { if (value) DefinirBatismo(true); }
    }

    public bool BatizadoNao
    {
        get => _foiBatizado == false;
        set { if (value) DefinirBatismo(false); }
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

    public ICommand CadastrarCommand { get; }

    public MembrosViewModel(MembroService membroService, AuthService auth)
    {
        _membroService = membroService;
        _auth = auth;

        _email = _auth.UsuarioLogado?.Email ?? string.Empty;

        CadastrarCommand = new Command(async () => await Cadastrar());
    }

    private void DefinirBatismo(bool? valor)
    {
        if (_foiBatizado == valor)
            return;

        _foiBatizado = valor;

        OnPropertyChanged(nameof(BatizadoSim));
        OnPropertyChanged(nameof(BatizadoNao));
    }

    private void LimparSucesso()
    {
        if (TemSucesso)
            MensagemSucesso = string.Empty;
    }

    private async Task Cadastrar()
    {
        if (_enviando)
            return;

        string? idAuth = _auth.UsuarioLogado?.IdAuth;

        if (string.IsNullOrEmpty(idAuth))
        {
            MensagemSucesso = string.Empty;
            MensagemErro = "Sua sessão terminou. Toque em Sair e entre de novo.";
            return;
        }

        List<string> erros = new();

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("Informe o nome.");
        else if (Nome.Trim().Length > 120)
            erros.Add("O nome está muito longo.");

        if (CpfValidador.SomenteDigitos(Telefone).Length < 10)
            erros.Add("Telefone inválido. Use (11) 99999-9999.");

        if (!string.IsNullOrWhiteSpace(CPF) && !CpfValidador.EhValido(CPF))
            erros.Add("CPF inválido.");

        if (!TentarConverterData(DataNascimentoTexto, out DateTime dataNascimento))
            erros.Add("Data de nascimento inválida. Use dd/mm/aaaa.");
        else if (dataNascimento.Date > DateTime.Today)
            erros.Add("A data de nascimento não pode estar no futuro.");

        if (!TentarConverterData(DataEntradaTexto, out DateTime dataEntrada))
            erros.Add("Data de entrada inválida. Use dd/mm/aaaa.");

        if (!Consentimento)
            erros.Add("Marque a caixa de concordância para continuar.");

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
            Membro? criado = await _membroService.Cadastrar(new Membro
            {
                Nome = Nome.Trim(),
                Telefone = Telefone.Trim(),
                CPF = CPF.Trim(),
                DataNascimento = dataNascimento,
                Email = Email.Trim(),
                CEP = CEP.Trim(),
                Endereco = Endereco.Trim(),
                Numero = Numero.Trim(),
                Complemento = Complemento.Trim(),
                Bairro = Bairro.Trim(),
                Cidade = Cidade.Trim(),
                Estado = Estado.Trim(),
                DataEntrada = dataEntrada,
                FoiBatizado = _foiBatizado,
                Observacao = Observacao.Trim(),
                Ativo = true,
                IdAuth = idAuth,
                Consentimento = true
            });

            criado ??= await _membroService.ObterPorAuthAsync(idAuth);

            if (criado == null)
            {
                MensagemErro = "Cadastro enviado, mas não foi possível abri-lo. Toque em Sair e entre de novo.";
                return;
            }

            await AbrirMinhaCarteirinha(criado);

            LimparFormulario();

            await Shell.Current.DisplayAlert("Sucesso", $"Membro {criado.Nome} cadastrado com sucesso!", "OK");
        }
        catch (Exception)
        {
            // Se a conta já tem cadastro (por exemplo, o envio anterior chegou), abre o existente.
            Membro? existente = null;

            try
            {
                existente = await _membroService.ObterPorAuthAsync(idAuth);
            }
            catch (Exception)
            {
            }

            if (existente != null)
                await AbrirMinhaCarteirinha(existente);
            else
                MensagemErro = "Não foi possível enviar. Verifique a internet e tente novamente.";
        }
        finally
        {
            _enviando = false;
        }
    }

    private static async Task AbrirMinhaCarteirinha(Membro membro)
    {
        await Shell.Current.GoToAsync(
            $"../{nameof(CarteirinhaMembro)}",
            new ShellNavigationQueryParameters
            {
            { "Membro", membro },
            { "Proprio", true }
            });
    }

    private void LimparFormulario()
    {
        Nome = string.Empty;
        Telefone = string.Empty;
        CPF = string.Empty;
        DataNascimentoTexto = string.Empty;
        Email = string.Empty;
        CEP = string.Empty;
        Endereco = string.Empty;
        Numero = string.Empty;
        Complemento = string.Empty;
        Bairro = string.Empty;
        Cidade = string.Empty;
        Estado = string.Empty;
        DataEntradaTexto = string.Empty;
        Observacao = string.Empty;

        DefinirBatismo(null);
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