using ChristMembers.Helpers;
using ChristMembers.Services;

namespace ChristMembers;

public partial class CriarContaPage : ContentPage
{
    private readonly AuthService _auth;
    private readonly MembroService _membros;
    private bool _criando;

    public CriarContaPage(AuthService auth, MembroService membros)
    {
        InitializeComponent();

        _auth = auth;
        _membros = membros;
    }

    private async void OnCriarClicked(object sender, EventArgs e)
    {
        if (_criando)
            return;

        string email = CampoEmail.Text?.Trim() ?? string.Empty;
        string senha = CampoSenha.Text ?? string.Empty;
        string confirmar = CampoConfirmar.Text ?? string.Empty;

        Aviso.IsVisible = false;

        if (email.Length == 0 || !email.Contains('@') || !email.Contains('.'))
        {
            MostrarMensagem("Informe um e-mail válido.");
            return;
        }

        if (senha.Length < 8)
        {
            MostrarMensagem("A senha precisa ter pelo menos 8 caracteres.");
            return;
        }

        if (senha != confirmar)
        {
            MostrarMensagem("As senhas não são iguais.");
            return;
        }

        _criando = true;
        BotaoCriar.IsEnabled = false;
        BotaoCriar.Text = "Criando...";
        Mensagem.IsVisible = false;

        try
        {
            ResultadoConta resultado = await _auth.CriarContaAsync(email, senha);

            if (!resultado.Sucesso)
            {
                MostrarMensagem(resultado.Mensagem);
                return;
            }

            if (resultado.JaEntrou)
            {
                // Fecha esta tela e o login e abre a área da pessoa.
                await AcessoHelper.EntrarNaAreaAsync(_auth, _membros, 2);
                return;
            }

            CampoSenha.Text = string.Empty;
            CampoConfirmar.Text = string.Empty;

            Aviso.Text = resultado.Mensagem;
            Aviso.IsVisible = true;
        }
        catch (Exception)
        {
            await _auth.SairAsync();

            MostrarMensagem("Não foi possível continuar. Verifique a internet e tente entrar.");
        }
        finally
        {
            _criando = false;
            BotaoCriar.IsEnabled = true;
            BotaoCriar.Text = "Criar conta";
        }
    }

    private void OnMostrarSenhaClicked(object sender, EventArgs e)
    {
        bool mostrar = CampoSenha.IsPassword; // se está oculta, vai mostrar

        CampoSenha.IsPassword = !mostrar;
        CampoConfirmar.IsPassword = !mostrar;

        BotaoOlho.Source = mostrar ? "eye_off.png" : "eye.png";
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void MostrarMensagem(string texto)
    {
        Mensagem.Text = texto;
        Mensagem.IsVisible = true;
    }
}