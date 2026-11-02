using System.Collections.ObjectModel;
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
    private const int LimiteOutros = 500;

    private readonly MembroService _membroService;
    private readonly AuthService _auth;
    private readonly ListasService _listas;
    private string _siglaCongregacao = string.Empty;

    private Membro? _membro;
    private bool _salvando;
    private bool _listasCarregadas;

    private bool? _foiBatizado;
    private bool? _temFilhos;
    private bool? _possuiDeficiencia;
    private bool _podeEditarDeficiencia;

    private OpcaoLista? _consagracaoSelecionada;
    private OpcaoLista? _setorSelecionado;
    private OpcaoLista? _estadoCivilSelecionado;
    private OpcaoLista? _generoSelecionado;
    private OpcaoLista? _deficienciaSelecionada;

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
    private string _textoDeficienciaSomenteLeitura = string.Empty;
    private string _mensagemErro = string.Empty;

    // ---------- Listas ----------

    public ObservableCollection<OpcaoLista> Consagracoes { get; } = new();
    public ObservableCollection<OpcaoLista> Setores { get; } = new();
    public ObservableCollection<OpcaoLista> EstadosCivis { get; } = new();
    public ObservableCollection<OpcaoLista> Generos { get; } = new();
    public ObservableCollection<OpcaoLista> Deficiencias { get; } = new();

    public OpcaoLista? ConsagracaoSelecionada
    {
        get => _consagracaoSelecionada;
        set => SetProperty(ref _consagracaoSelecionada, value);
    }

    public OpcaoLista? SetorSelecionado
    {
        get => _setorSelecionado;
        set => SetProperty(ref _setorSelecionado, value);
    }

    public OpcaoLista? EstadoCivilSelecionado
    {
        get => _estadoCivilSelecionado;
        set => SetProperty(ref _estadoCivilSelecionado, value);
    }

    public OpcaoLista? GeneroSelecionado
    {
        get => _generoSelecionado;
        set => SetProperty(ref _generoSelecionado, value);
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

    // ---------- Dados ----------

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

    public string SiglaCongregacao
    {
        get => _siglaCongregacao;
        set => SetProperty(ref _siglaCongregacao, value ?? string.Empty);
    }

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

    public bool BatizadoNaoInformado
    {
        get => _foiBatizado == null;
        set { if (value) DefinirBatismo(null); }
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

    public bool FilhosNaoInformado
    {
        get => _temFilhos == null;
        set { if (value) DefinirFilhos(null); }
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

    public bool PodeEditarDeficiencia => _podeEditarDeficiencia;

    public bool MostrarDeficienciaSomenteLeitura => !_podeEditarDeficiencia;

    public bool MostrarTipoDeficiencia => _podeEditarDeficiencia && _possuiDeficiencia == true;

    public bool MostrarOutros => MostrarTipoDeficiencia && DeficienciaSelecionada?.Outros == true;

    public string TextoDeficienciaSomenteLeitura => _textoDeficienciaSomenteLeitura;

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

    public EditarMembroViewModel(MembroService membroService, AuthService auth, ListasService listas)
    {
        _membroService = membroService;
        _auth = auth;
        _listas = listas;

        SalvarCommand = new Command(async () => await Salvar());
        CancelarCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    // ---------- Carregar ----------

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
        _siglaCongregacao = membro.SiglaCongregacao ?? string.Empty;

        _foiBatizado = membro.FoiBatizado;
        _temFilhos = membro.TemFilhos;
        _possuiDeficiencia = membro.PossuiDeficiencia;

        _dataNascimentoTexto = FormatarData(membro.DataNascimento);
        _dataEntradaTexto = FormatarData(membro.DataEntrada);
        _dataBatismoTexto = FormatarData(membro.DataBatismo);

        _deficienciaOutros = membro.DeficienciaOutros ?? string.Empty;

        // Só dá para editar a deficiência quando o próprio membro já a informou, com a autorização.
        _podeEditarDeficiencia = membro.PossuiDeficiencia == true && membro.ConsentimentoSaude;

        _textoDeficienciaSomenteLeitura = membro.PossuiDeficiencia switch
        {
            false => "Deficiência: não. Só o próprio membro pode informar esse dado, com a autorização dele.",
            null => "Deficiência: não informada. Só o próprio membro pode informar esse dado, com a autorização dele.",
            _ => "Deficiência informada sem a autorização registrada. Não é possível editar."
        };

        _mensagemErro = string.Empty;

        SelecionarListas();

        OnPropertyChanged(string.Empty);
    }

    public async Task CarregarListasAsync()
    {
        if (_listasCarregadas)
            return;

        try
        {
            List<Consagracao> consagracoes = await _listas.ObterConsagracoesAsync();
            List<Setor> setores = await _listas.ObterSetoresAsync();
            List<EstadoCivil> estados = await _listas.ObterEstadosCivisAsync();
            List<Genero> generos = await _listas.ObterGenerosAsync();
            List<Deficiencia> deficiencias = await _listas.ObterDeficienciasAsync();

            Consagracoes.Clear();
            Consagracoes.Add(OpcaoLista.Nenhuma());

            foreach (Consagracao c in consagracoes)
                Consagracoes.Add(new OpcaoLista { Id = c.Id, Nome = c.Descricao });

            Setores.Clear();
            Setores.Add(OpcaoLista.Nenhuma());

            foreach (Setor s in setores)
                Setores.Add(new OpcaoLista { Id = s.Id, Nome = s.Descricao });

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
            MensagemErro = "Não foi possível carregar as listas (consagração, setor, estado civil, gênero e deficiência). O que já estava no cadastro será mantido.";
        }

        SelecionarListas();
    }

    private void SelecionarListas()
    {
        if (_membro == null || !_listasCarregadas)
            return;

        _consagracaoSelecionada =
            Consagracoes.FirstOrDefault(o => o.Id == _membro.IdConsagracao)
            ?? Consagracoes.FirstOrDefault();

        _setorSelecionado =
            Setores.FirstOrDefault(o => o.Id == _membro.IdSetor)
            ?? Setores.FirstOrDefault();

        _estadoCivilSelecionado =
            EstadosCivis.FirstOrDefault(o => o.Id == _membro.IdEstadoCivil)
            ?? EstadosCivis.FirstOrDefault();

        _generoSelecionado =
            Generos.FirstOrDefault(o => o.Id == _membro.IdGenero)
            ?? Generos.FirstOrDefault();

        _deficienciaSelecionada = _membro.IdDeficiencia.HasValue
            ? Deficiencias.FirstOrDefault(o => o.Id == _membro.IdDeficiencia)
            : null;

        OnPropertyChanged(string.Empty);
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
        OnPropertyChanged(nameof(BatizadoNaoInformado));
        OnPropertyChanged(nameof(MostrarDataBatismo));
    }

    private void DefinirFilhos(bool? valor)
    {
        if (_temFilhos == valor)
            return;

        _temFilhos = valor;

        OnPropertyChanged(nameof(FilhosSim));
        OnPropertyChanged(nameof(FilhosNao));
        OnPropertyChanged(nameof(FilhosNaoInformado));
    }

    private void DefinirDeficiencia(bool? valor)
    {
        if (!_podeEditarDeficiencia || _possuiDeficiencia == valor)
            return;

        _possuiDeficiencia = valor;

        if (valor != true)
        {
            _deficienciaSelecionada = null;
            _deficienciaOutros = string.Empty;
        }

        OnPropertyChanged(nameof(DeficienciaSim));
        OnPropertyChanged(nameof(DeficienciaNao));
        OnPropertyChanged(nameof(DeficienciaSelecionada));
        OnPropertyChanged(nameof(DeficienciaOutros));
        OnPropertyChanged(nameof(ContadorOutros));
        OnPropertyChanged(nameof(MostrarTipoDeficiencia));
        OnPropertyChanged(nameof(MostrarOutros));
    }

    // ---------- Salvar ----------

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
                erros.Add("Data de filiação inválida. Use dd/mm/aaaa.");
            else
                entrada = dataEntrada;
        }

        DateTime? batismo = null;

        if (_foiBatizado == true && !string.IsNullOrWhiteSpace(DataBatismoTexto))
        {
            if (!TentarConverterData(DataBatismoTexto, out DateTime dataBatismo))
                erros.Add("Data do batismo inválida. Use dd/mm/aaaa.");
            else if (dataBatismo.Date > DateTime.Today)
                erros.Add("A data do batismo não pode estar no futuro.");
            else if (nascimento.HasValue && dataBatismo.Date < nascimento.Value.Date)
                erros.Add("A data do batismo não pode ser anterior ao nascimento.");
            else
                batismo = dataBatismo;
        }

        bool editarDeficiencia = _podeEditarDeficiencia && _listasCarregadas;

        if (editarDeficiencia && _possuiDeficiencia == true)
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
        }

        if (SiglaCongregacao.Trim().Length > 10)
            erros.Add("A sigla da congregação pode ter no máximo 10 caracteres.");

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
            _membro.DataNascimento = nascimento;
            _membro.Email = Email.Trim();
            _membro.CEP = CEP.Trim();
            _membro.Endereco = Endereco.Trim();
            _membro.Numero = Numero.Trim();
            _membro.Complemento = Complemento.Trim();
            _membro.Bairro = Bairro.Trim();
            _membro.Cidade = Cidade.Trim();
            _membro.Estado = Estado.Trim();
            _membro.DataEntrada = entrada;
            _membro.FoiBatizado = _foiBatizado;
            _membro.DataBatismo = _foiBatizado == true ? batismo : null;
            _membro.TemFilhos = _temFilhos;
            _membro.Observacao = Observacao.Trim();
            _membro.SiglaCongregacao = string.IsNullOrWhiteSpace(SiglaCongregacao) ? null : SiglaCongregacao.Trim().ToUpperInvariant();

            // Só mexe nas listas se elas carregaram: assim nada é apagado por engano.
            if (_listasCarregadas)
            {
                _membro.IdConsagracao = ConsagracaoSelecionada?.Id;
                _membro.IdSetor = SetorSelecionado?.Id;
                _membro.IdEstadoCivil = EstadoCivilSelecionado?.Id;
                _membro.IdGenero = GeneroSelecionado?.Id;
            }

            if (editarDeficiencia)
            {
                if (_possuiDeficiencia == true)
                {
                    _membro.PossuiDeficiencia = true;
                    _membro.IdDeficiencia = DeficienciaSelecionada?.Id;
                    _membro.DeficienciaOutros = MostrarOutros ? DeficienciaOutros.Trim() : null;
                }
                else
                {
                    // Remover a informação também apaga a autorização de saúde.
                    _membro.PossuiDeficiencia = false;
                    _membro.IdDeficiencia = null;
                    _membro.DeficienciaOutros = null;
                    _membro.ConsentimentoSaude = false;
                }
            }

            await _membroService.Atualizar(_membro);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível salvar. Verifique a internet e tente novamente.";
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