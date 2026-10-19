using ChristMembers.Services;

namespace ChristMembers;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _auth;
    private bool _primeiroAcesso;

    public LoginPage(AuthService auth)
    {
        InitializeComponent();

        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _auth.Sair();

        _primeiroAcesso = !await _auth.ExisteUsuario();

        ConfirmarSenha.IsVisible = _primeiroAcesso;
        //AreaCadastro.IsVisible = !_primeiroAcesso;
        Mensagem.IsVisible = false;

        if (_primeiroAcesso)
        {
            Titulo.Text = "Primeiro acesso";
            Subtitulo.Text = "Use o usuário admin e escolha uma senha com pelo menos 6 caracteres.";
            BotaoEntrar.Text = "Criar senha";
        }
        else
        {
            Titulo.Text = "Acesso de membros";
            Subtitulo.Text = "Entre com o seu usuário e senha.";
            BotaoEntrar.Text = "Entrar";
        }
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        string usuario = Usuario.Text?.Trim() ?? string.Empty;
        string senha = Senha.Text ?? string.Empty;

        if (_primeiroAcesso)
        {
            if (!string.Equals(usuario, AuthService.UsuarioAdmin, StringComparison.OrdinalIgnoreCase))
            {
                MostrarMensagem("No primeiro acesso, o usuário deve ser admin.");
                return;
            }

            if (senha.Length < 6)
            {
                MostrarMensagem("A senha precisa ter pelo menos 6 caracteres.");
                return;
            }

            if (senha != ConfirmarSenha.Text)
            {
                MostrarMensagem("As senhas não são iguais.");
                return;
            }

            await _auth.CriarUsuario(usuario, senha, AuthService.CargoAdmin);
        }

        if (await _auth.Validar(usuario, senha) == null)
        {
            MostrarMensagem("Usuário ou senha incorretos.");
            return;
        }

        LimparCampos();

        await Shell.Current.GoToAsync(nameof(PainelAdmin));
    }

    private void OnLimparClicked(object sender, EventArgs e)
    {
        LimparCampos();
    }

    private void MostrarMensagem(string texto)
    {
        Mensagem.Text = texto;
        Mensagem.IsVisible = true;
    }

    private void LimparCampos()
    {
        Usuario.Text = string.Empty;
        Senha.Text = string.Empty;
        ConfirmarSenha.Text = string.Empty;
        Mensagem.IsVisible = false;
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroMembro));
    }
}