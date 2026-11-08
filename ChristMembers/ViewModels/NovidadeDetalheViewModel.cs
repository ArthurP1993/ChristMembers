using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class NovidadeDetalheViewModel : INotifyPropertyChanged
{
    private const int LimiteAcompanhantes = 10;

    private static readonly Color Azul = Color.FromArgb("#1B2F7A");
    private static readonly Color AzulClaro = Color.FromArgb("#E4EBFF");

    private readonly ConfirmacaoService _confirmacoes;
    private readonly MembroService _membros;
    private readonly AuthService _auth;

    private Novidade? _novidade;
    private Membro? _meuCadastro;
    private ConfirmacaoPresenca? _minhaConfirmacao;

    private long? _confirmacaoId;
    private string _confirmacaoToken = string.Empty;
    private string _nomeConfirmado = string.Empty;
    private List<string> _acompanhantes = new();

    private bool _abaMembro = true;
    private bool _loginFeitoAqui;
    private bool _adminAoAbrir;
    private bool _ocupado;

    private string _nome = string.Empty;
    private string _telefone = string.Empty;
    private string _emailLogin = string.Empty;
    private string _senhaLogin = string.Empty;
    private string _mensagemErro = string.Empty;

    public Novidade? Novidade => _novidade;

    // ---------- Campos ----------

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

    public string EmailLogin
    {
        get => _emailLogin;
        set => SetProperty(ref _emailLogin, value ?? string.Empty);
    }

    public string SenhaLogin
    {
        get => _senhaLogin;
        set => SetProperty(ref _senhaLogin, value ?? string.Empty);
    }

    public string NomeMembro => _meuCadastro?.Nome ?? string.Empty;

    public string TextoConfirmadoLogado => $"Presença confirmada para {_nomeConfirmado}.";

    public string TextoConfirmadoVisitante => $"Obrigado, {_nomeConfirmado}! Esperamos você.";

    public bool TemAcompanhantes => _acompanhantes.Count > 0;

    public string TextoAcompanhantes => _acompanhantes.Count == 1
        ? $"Acompanhante: {_acompanhantes[0]}"
        : $"Acompanhantes: {string.Join(", ", _acompanhantes)}";

    public bool PodeAdicionarAcompanhantes => _acompanhantes.Count < LimiteAcompanhantes;

    public string TextoBotaoAcompanhantes => TemAcompanhantes
        ? "Adicionar mais acompanhantes"
        : "Adicionar acompanhantes";

    public string MensagemErro
    {
        get => _mensagemErro;
        set => SetProperty(ref _mensagemErro, value ?? string.Empty);
    }

    public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

    // ---------- O que mostrar ----------

    private bool Logado => _auth.UsuarioLogado != null;

    private bool MembroPronto =>
        _meuCadastro != null
        && CpfValidador.SomenteDigitos(_meuCadastro.Telefone).Length is 10 or 11;

    private bool Concluido => _confirmacaoId.HasValue;

    public bool PodeConfirmar => _novidade?.PodeConfirmar == true;

    public bool MostrarAbas => PodeConfirmar && !Concluido && !Logado;

    public bool MostrarLogin => MostrarAbas && _abaMembro;

    public bool MostrarFormulario =>
        PodeConfirmar && !Concluido
        && ((!Logado && !_abaMembro) || (Logado && !MembroPronto));

    public bool MostrarPainelMembro =>
        PodeConfirmar && !Concluido && Logado && MembroPronto;

    public bool MostrarConfirmadoLogado => PodeConfirmar && Concluido && Logado;

    public bool MostrarConfirmadoVisitante => PodeConfirmar && Concluido && !Logado;

    public bool MostrarAdmin => _adminAoAbrir && _novidade?.PermiteConfirmacao == true;

    public Color CorFundoAbaMembro => _abaMembro ? Azul : AzulClaro;
    public Color CorTextoAbaMembro => _abaMembro ? Colors.White : Azul;
    public Color CorFundoAbaVisitante => _abaMembro ? AzulClaro : Azul;
    public Color CorTextoAbaVisitante => _abaMembro ? Azul : Colors.White;

    // ---------- Comandos ----------

    public ICommand AbaMembroCommand { get; }
    public ICommand AbaVisitanteCommand { get; }
    public ICommand EntrarCommand { get; }
    public ICommand ConfirmarMembroCommand { get; }
    public ICommand ConfirmarFormularioCommand { get; }
    public ICommand CancelarCommand { get; }
    public ICommand OutraPessoaCommand { get; }
    public ICommand AdicionarAcompanhantesCommand { get; }
    public ICommand VerConfirmadosCommand { get; }

    public NovidadeDetalheViewModel(
        ConfirmacaoService confirmacoes,
        MembroService membros,
        AuthService auth)
    {
        _confirmacoes = confirmacoes;
        _membros = membros;
        _auth = auth;

        AbaMembroCommand = new Command(() => TrocarAba(true));
        AbaVisitanteCommand = new Command(() => TrocarAba(false));
        EntrarCommand = new Command(async () => await Entrar());
        ConfirmarMembroCommand = new Command(async () => await ConfirmarComoMembro());
        ConfirmarFormularioCommand = new Command(async () => await ConfirmarComFormulario());
        CancelarCommand = new Command(async () => await Cancelar());
        OutraPessoaCommand = new Command(OutraPessoa);
        AdicionarAcompanhantesCommand = new Command(async () => await AbrirAcompanhantes());
        VerConfirmadosCommand = new Command(async () => await VerConfirmados());
    }

    // ---------- Ciclo de vida ----------

    public void Definir(Novidade novidade)
    {
        _novidade = novidade;
        _adminAoAbrir = _auth.UsuarioLogado?.Cargo == AuthService.CargoAdmin;

        Atualizar();
    }

    public async Task AtualizarAsync()
    {
        if (PodeConfirmar && Logado)
            await CarregarDadosDoLoginAsync();

        Atualizar();
    }

    // Quem entrou por esta tela é desconectado ao sair dela.
    public async Task EncerrarLoginAsync()
    {
        if (!_loginFeitoAqui)
            return;

        _loginFeitoAqui = false;

        await _auth.SairAsync();
    }

    private async Task CarregarDadosDoLoginAsync()
    {
        string? idAuth = _auth.UsuarioLogado?.IdAuth;

        if (string.IsNullOrEmpty(idAuth) || _novidade == null)
            return;

        try
        {
            _meuCadastro = await _membros.ObterPorAuthAsync(idAuth);
            _minhaConfirmacao = await _confirmacoes.ObterMinhaAsync(_novidade.Id, idAuth);

            if (_minhaConfirmacao != null)
            {
                _confirmacaoId = _minhaConfirmacao.Id;
                _confirmacaoToken = _minhaConfirmacao.Token ?? string.Empty;
                _nomeConfirmado = _minhaConfirmacao.Nome;
                _acompanhantes = await _confirmacoes.ListarAcompanhantesAsync(_minhaConfirmacao.Id);
            }
            else
            {
                LimparConfirmacao();
            }
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível carregar os seus dados. Verifique a internet.";
        }
    }

    // ---------- Ações ----------

    private void TrocarAba(bool membro)
    {
        _abaMembro = membro;
        MensagemErro = string.Empty;

        Atualizar();
    }

    private void OutraPessoa()
    {
        LimparConfirmacao();
        MensagemErro = string.Empty;

        Atualizar();
    }

    private void LimparConfirmacao()
    {
        _minhaConfirmacao = null;
        _confirmacaoId = null;
        _confirmacaoToken = string.Empty;
        _nomeConfirmado = string.Empty;
        _acompanhantes = new List<string>();
    }

    private async Task Entrar()
    {
        if (_ocupado)
            return;

        string email = EmailLogin.Trim();

        if (email.Length == 0 || SenhaLogin.Length == 0)
        {
            MensagemErro = "Informe o e-mail e a senha.";
            Atualizar();
            return;
        }

        _ocupado = true;
        MensagemErro = string.Empty;

        try
        {
            string? erro = await _auth.EntrarAsync(email, SenhaLogin);

            if (erro != null)
            {
                MensagemErro = erro;
                return;
            }

            _loginFeitoAqui = true;
            SenhaLogin = string.Empty;

            await CarregarDadosDoLoginAsync();
        }
        finally
        {
            _ocupado = false;

            Atualizar();
        }
    }

    private async Task ConfirmarComoMembro()
    {
        if (_meuCadastro == null || !MembroPronto)
            return;

        await Enviar(
            _meuCadastro.Nome,
            CpfValidador.SomenteDigitos(_meuCadastro.Telefone));
    }

    private async Task ConfirmarComFormulario()
    {
        string telefone = CpfValidador.SomenteDigitos(Telefone);

        List<string> erros = new();

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("Informe o seu nome.");
        else if (Nome.Trim().Length > 120)
            erros.Add("O nome está muito longo.");

        if (telefone.Length is not (10 or 11))
            erros.Add("Telefone inválido. Use (11) 99999-9999.");

        if (erros.Count > 0)
        {
            MensagemErro = string.Join(Environment.NewLine, erros);
            Atualizar();
            return;
        }

        await Enviar(Nome.Trim(), telefone);
    }

    private async Task Enviar(string nome, string telefone)
    {
        if (_ocupado || _novidade == null)
            return;

        _ocupado = true;
        MensagemErro = string.Empty;

        bool confirmou = false;

        try
        {
            RetornoConfirmacao retorno = await _confirmacoes.ConfirmarAsync(_novidade.Id, nome, telefone);

            _confirmacaoId = retorno.Id;
            _confirmacaoToken = retorno.Token;
            _nomeConfirmado = nome;
            _acompanhantes = new List<string>();

            string? idAuth = _auth.UsuarioLogado?.IdAuth;

            if (!string.IsNullOrEmpty(idAuth))
            {
                try
                {
                    _minhaConfirmacao = await _confirmacoes.ObterMinhaAsync(_novidade.Id, idAuth);
                }
                catch (Exception)
                {
                }
            }

            Nome = string.Empty;
            Telefone = string.Empty;

            confirmou = true;
        }
        catch (Exception ex)
        {
            MensagemErro = TraduzirErro(ex);
        }
        finally
        {
            _ocupado = false;

            Atualizar();
        }

        if (confirmou)
            await PerguntarAcompanhantes();
    }

    private async Task PerguntarAcompanhantes()
    {
        bool levaMais = await Shell.Current.DisplayAlert(
            "Presença confirmada ✓",
            "Você vai levar mais pessoas?",
            "Sim",
            "Não");

        if (levaMais)
            await AbrirAcompanhantes();
    }

    private async Task AbrirAcompanhantes()
    {
        if (_confirmacaoId == null || _novidade == null)
            return;

        Action<List<string>> aoConcluir = nomes =>
        {
            _acompanhantes.AddRange(nomes);

            Atualizar();
        };

        await Shell.Current.GoToAsync(
            nameof(AcompanhantesPage),
            new ShellNavigationQueryParameters
            {
                { "IdConfirmacao", _confirmacaoId.Value },
                { "Token", _confirmacaoToken },
                { "Titulo", _novidade.Titulo },
                { "Existentes", _acompanhantes.Count },
                { "AoConcluir", aoConcluir }
            });
    }

    private async Task Cancelar()
    {
        if (_minhaConfirmacao == null || _ocupado)
            return;

        bool confirmou = await Shell.Current.DisplayAlert(
            "Cancelar presença",
            "Deseja cancelar a sua confirmação? Os acompanhantes também serão retirados.",
            "Cancelar presença",
            "Voltar");

        if (!confirmou)
            return;

        _ocupado = true;

        try
        {
            await _confirmacoes.CancelarAsync(_minhaConfirmacao);

            LimparConfirmacao();
            MensagemErro = string.Empty;
        }
        catch (Exception)
        {
            MensagemErro = "Não foi possível cancelar. Verifique a internet.";
        }
        finally
        {
            _ocupado = false;

            Atualizar();
        }
    }

    private async Task VerConfirmados()
    {
        if (_novidade == null || !_adminAoAbrir)
            return;

        await Shell.Current.GoToAsync(
            nameof(ConfirmadosPage),
            new ShellNavigationQueryParameters
            {
                { "Novidade", _novidade }
            });
    }

    private static string TraduzirErro(Exception ex)
    {
        string texto = ex.Message;

        if (texto.Contains("23505") || texto.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            return "Esta presença já foi confirmada (mesmo telefone ou mesma conta).";

        if (texto.Contains("22023"))
            return "Confira o nome e o telefone e tente de novo.";

        if (texto.Contains("42501")
            || texto.Contains("row-level security", StringComparison.OrdinalIgnoreCase)
            || texto.Contains("encerradas", StringComparison.OrdinalIgnoreCase))
        {
            return "As confirmações deste evento estão encerradas.";
        }

        return "Não foi possível confirmar. Verifique a internet e tente novamente.";
    }

    // Reavalia todos os campos da tela de uma vez.
    private void Atualizar()
    {
        OnPropertyChanged(string.Empty);
        OnPropertyChanged(nameof(TemErro));
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