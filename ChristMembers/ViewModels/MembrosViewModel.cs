using System.Collections.ObjectModel;
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
    private const int LimiteOutros = 500;

    private readonly MembroService _membroService;
    private readonly AuthService _auth;
    private readonly ListasService _listas;

    private bool _listasCarregadas;
    private bool _consentimento;
    private bool _consentimentoSaude;

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
    private string _dataBatismoTexto = string.Empty;
    private string _observacao = string.Empty;
    private string _deficienciaOutros = string.Empty;

    private bool? _foiBatizado;
    private bool? _temFilhos;
    private bool? _possuiDeficiencia;

    private OpcaoLista? _generoSelecionado;
    private OpcaoLista? _estadoCivilSelecionado;
    private OpcaoLista? _deficienciaSelecionada;

    private string _mensagemErro = string.Empty;
    private string _mensagemSucesso = string.Empty;
    private bool _enviando;

    // ---------- Listas ----------

    public ObservableCollection<OpcaoLista> Generos { get; } = new();
    public ObservableCollection<OpcaoLista> EstadosCivis { get; } = new();
    public ObservableCollection<OpcaoLista> Deficiencias { get; } = new();

    public OpcaoLista? GeneroSelecionado
    {
        get => _generoSelecionado;
        set => SetProperty(ref _generoSelecionado, value);
    }

    public OpcaoLista? EstadoCivilSelecionado
    {
        get => _estadoCivilSelecionado;
        set => SetProperty(ref _estadoCivilSelecionado, value);
    }

    public OpcaoLista? DeficienciaSelecionada
    {
        get => _deficienciaSelecionada;

        set
        {
            if (!SetProperty(ref _deficienciaSelecionada, value))
                return;

            if (value?.Outros != true)
                DeficienciaOutros = string.Empty;

            OnPropertyChanged(nameof(MostrarOutros));
        }
    }

    // ---------- Consentimentos ----------

    public bool Consentimento
    {
        get => _consentimento;
        set => SetProperty(ref _consentimento, value);
    }

    public bool ConsentimentoSaude
    {
        get => _consentimentoSaude;
        set => SetProperty(ref _consentimentoSaude, value);
    }

    // ---------- Dados ----------

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

    public string DataBatismoTexto
    {
        get => _dataBatismoTexto;
        set => SetProperty(ref _dataBatismoTexto, value ?? string.Empty);
    }

    public string Observacao
    {
        get => _observacao;
        set => SetProperty(ref _observacao, value ?? string.Empty);
    }

    public string DeficienciaOutros
    {
        get => _deficienciaOutros;

        set
        {
            if (SetProperty(ref _deficienciaOutros, value ?? string.Empty))
                OnPropertyChanged(nameof(ContadorOutros));
        }
    }

    public string ContadorOutros => $"{DeficienciaOutros.Length}/{LimiteOutros}";

    // ---------- Sim / Não ----------

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

    public bool FilhosSim
    {
        get => _temFilhos == true;
        set { if (value) DefinirFilhos(true); }
    }

    public bool FilhosNao
    {
        get => _temFilhos == false;
        set { if (value) DefinirFilhos(false); }
    }

    public bool DeficienciaSim
    {
        get => _possuiDeficiencia == true;
        set { if (value) DefinirDeficiencia(true); }
    }

    public bool DeficienciaNao
    {
        get => _possuiDeficiencia == false;
        set { if (value) DefinirDeficiencia(false); }
    }

    // ---------- O que aparece na tela ----------

    public bool MostrarDataBatismo => _foiBatizado == true;

    public bool MostrarDeficiencia => _possuiDeficiencia == true;

    public bool MostrarOutros =>
        _possuiDeficiencia == true && DeficienciaSelecionada?.Outros == true;

    // ---------- Mensagens ----------

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

    public MembrosViewModel(MembroService membroService, AuthService auth, ListasService listas)
    {
        _membroService = membroService;
        _auth = auth;
        _listas = listas;

        _email = _auth.UsuarioLogado?.Email ?? string.Empty;

        CadastrarCommand = new Command(async () => await Cadastrar());
    }

    // ---------- Listas: carregar ----------

    public async Task CarregarListasAsync()
    {
        if (_listasCarregadas)
            return;

        try
        {
            List<EstadoCivil> estados = await _listas.ObterEstadosCivisAsync();
            List<Genero> generos = await _listas.ObterGenerosAsync();
            List<Deficiencia> deficiencias = await _listas.ObterDeficienciasAsync();

            EstadosCivis.Clear();
            EstadosCivis.Add(OpcaoLista.Nenhuma());

            foreach (EstadoCivil e in estados)
                EstadosCivis.Add(new OpcaoLista { Id = e.Id, Nome = e.Descricao });

            Generos.Clear();
            Generos.Add(OpcaoLista.Nenhuma());

            foreach (Genero g in generos)
                Generos.Add(new OpcaoLista { Id = g.Id, Nome = g.Descricao });

            Deficiencias.Clear();

            foreach (Deficiencia d in deficiencias)
                Deficiencias.Add(new OpcaoLista { Id = d.Id, Nome = d.Descricao, Outros = d.Outros });

            _listasCarregadas = true;
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível carregar as listas (gênero, estado civil e deficiência). Verifique a internet e abra a tela de novo.";
        }
    }

    // ---------- Sim / Não: lógica ----------

    private void DefinirBatismo(bool? valor)
    {
        if (_foiBatizado == valor)
            return;

        _foiBatizado = valor;

        if (valor != true)
            DataBatismoTexto = string.Empty;

        OnPropertyChanged(nameof(BatizadoSim));
        OnPropertyChanged(nameof(BatizadoNao));
        OnPropertyChanged(nameof(MostrarDataBatismo));
    }

    private void DefinirFilhos(bool? valor)
    {
        if (_temFilhos == valor)
            return;

        _temFilhos = valor;

        OnPropertyChanged(nameof(FilhosSim));
        OnPropertyChanged(nameof(FilhosNao));
    }

    private void DefinirDeficiencia(bool? valor)
    {
        if (_possuiDeficiencia == valor)
            return;

        _possuiDeficiencia = valor;

        if (valor != true)
        {
            DeficienciaSelecionada = null;
            DeficienciaOutros = string.Empty;
            ConsentimentoSaude = false;
        }

        OnPropertyChanged(nameof(DeficienciaSim));
        OnPropertyChanged(nameof(DeficienciaNao));
        OnPropertyChanged(nameof(MostrarDeficiencia));
        OnPropertyChanged(nameof(MostrarOutros));
    }

    private void LimparSucesso()
    {
        if (TemSucesso)
            MensagemSucesso = string.Empty;
    }

    // ---------- Cadastrar ----------

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

        bool nascimentoOk = TentarConverterData(DataNascimentoTexto, out DateTime dataNascimento);

        if (!nascimentoOk)
            erros.Add("Data de nascimento inválida. Use dd/mm/aaaa.");
        else if (dataNascimento.Date > DateTime.Today)
            erros.Add("A data de nascimento não pode estar no futuro.");

        if (!TentarConverterData(DataEntradaTexto, out DateTime dataEntrada))
            erros.Add("Data de filiação inválida. Use dd/mm/aaaa.");

        DateTime? dataBatismo = null;

        if (_foiBatizado == true && !string.IsNullOrWhiteSpace(DataBatismoTexto))
        {
            if (!TentarConverterData(DataBatismoTexto, out DateTime batismo))
                erros.Add("Data do batismo inválida. Use dd/mm/aaaa.");
            else if (batismo.Date > DateTime.Today)
                erros.Add("A data do batismo não pode estar no futuro.");
            else if (nascimentoOk && batismo.Date < dataNascimento.Date)
                erros.Add("A data do batismo não pode ser anterior ao nascimento.");
            else
                dataBatismo = batismo;
        }

        if (_possuiDeficiencia == true)
        {
            if (DeficienciaSelecionada?.Id == null)
            {
                erros.Add("Escolha o tipo de deficiência.");
            }
            else if (MostrarOutros)
            {
                string descricao = DeficienciaOutros.Trim();

                if (descricao.Length == 0)
                    erros.Add("Descreva a deficiência.");
                else if (descricao.Length > LimiteOutros)
                    erros.Add("A descrição da deficiência pode ter no máximo 500 caracteres.");
            }

            if (!ConsentimentoSaude)
                erros.Add("Marque a autorização sobre a informação de saúde.");
        }

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
            bool temDeficiencia = _possuiDeficiencia == true;

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
                DataBatismo = _foiBatizado == true ? dataBatismo : null,
                Observacao = Observacao.Trim(),
                IdEstadoCivil = EstadoCivilSelecionado?.Id,
                TemFilhos = _temFilhos,
                IdGenero = GeneroSelecionado?.Id,
                PossuiDeficiencia = _possuiDeficiencia,
                IdDeficiencia = temDeficiencia ? DeficienciaSelecionada?.Id : null,
                DeficienciaOutros = temDeficiencia && MostrarOutros
                    ? DeficienciaOutros.Trim()
                    : null,
                ConsentimentoSaude = temDeficiencia && ConsentimentoSaude,
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

        GeneroSelecionado = null;
        EstadoCivilSelecionado = null;

        DefinirBatismo(null);
        DefinirFilhos(null);
        DefinirDeficiencia(null);
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