using ChristMembers.Services;

namespace ChristMembers;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _auth;
    private bool _entrando;

    public LoginPage(AuthService auth)
    {
        InitializeComponent();

        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Mensagem.IsVisible = false;

        await _auth.SairAsync();
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        if (_entrando)
            return;

        string email = CampoEmail.Text?.Trim() ?? string.Empty;
        string senha = CampoSenha.Text ?? string.Empty;

        if (email.Length == 0 || senha.Length == 0)
        {
            MostrarMensagem("Informe o e-mail e a senha.");
            return;
        }

        _entrando = true;
        BotaoEntrar.IsEnabled = false;
        BotaoEntrar.Text = "Entrando...";

        try
        {
            string? erro = await _auth.EntrarAsync(email, senha);

            if (erro != null)
            {
                MostrarMensagem(erro);
                return;
            }

            if (_auth.UsuarioLogado?.Cargo != AuthService.CargoAdmin)
            {
                await _auth.SairAsync();

                MostrarMensagem("Este acesso não tem permissão de administrador.");
                return;
            }

            LimparCampos();

            await Shell.Current.GoToAsync(nameof(PainelAdmin));
        }
        finally
        {
            _entrando = false;
            BotaoEntrar.IsEnabled = true;
            BotaoEntrar.Text = "Entrar";
        }
    }

    private void OnLimparClicked(object sender, EventArgs e)
    {
        LimparCampos();
    }

    private void OnMostrarSenhaClicked(object sender, EventArgs e)
    {
        bool mostrar = CampoSenha.IsPassword; // se está oculta, vai mostrar

        int posicao = CampoSenha.CursorPosition;

        CampoSenha.IsPassword = !mostrar;

        CampoSenha.CursorPosition = posicao;

        BotaoOlho.Source = mostrar ? "eye_off.png" : "eye.png";
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroMembro));
    }

    private void MostrarMensagem(string texto)
    {
        Mensagem.Text = texto;
        Mensagem.IsVisible = true;
    }

    private void LimparCampos()
    {
        CampoEmail.Text = string.Empty;
        CampoSenha.Text = string.Empty;
        Mensagem.IsVisible = false;

        CampoSenha.IsPassword = true;
        BotaoOlho.Source = "eye.png";
    }
}