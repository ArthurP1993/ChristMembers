using ChristMembers.Models;
using ChristMembers.Services;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class CarteirinhaMembro : ContentPage, IQueryAttributable
{
    private readonly AuthService _auth;
    private readonly FotoMembroService _fotos;

    private Membro? _membro;
    private bool _proprio;
    private bool _trocandoFoto;

    public CarteirinhaMembro(AuthService auth, FotoMembroService fotos)
    {
        InitializeComponent();

        _auth = auth;
        _fotos = fotos;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Membro", out object? valor) && valor is Membro membro)
        {
            _membro = membro;

            BindingContext = new CarteirinhaViewModel(membro);
        }

        if (query.TryGetValue("Proprio", out object? proprio) && proprio is bool ehProprio && ehProprio)
        {
            ConfigurarModoProprio();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_membro == null)
            return;

        BindingContext = new CarteirinhaViewModel(_membro);

        await CarregarFotoAsync();
    }

    // Quem vê a própria carteirinha só sai pelo botão Sair.
    protected override bool OnBackButtonPressed()
    {
        return _proprio;
    }

    private void ConfigurarModoProprio()
    {
        if (_proprio)
            return;

        _proprio = true;

        BotaoEditar.IsVisible = false;
        BotaoFoto.IsVisible = true;

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Sair",
            Command = new Command(async () => await SairAsync())
        });
    }

    private async Task CarregarFotoAsync()
    {
        if (_membro == null || BindingContext is not CarteirinhaViewModel vm)
            return;

        try
        {
            vm.FotoUrl = await _fotos.ObterUrlAsync(_membro);
        }
        catch (Exception)
        {
            vm.FotoUrl = null;
        }
    }

    private async Task SairAsync()
    {
        await _auth.SairAsync();

        await Shell.Current.GoToAsync("..");
    }

    private void OnDadosClicked(object sender, EventArgs e)
    {
        PainelDados.IsVisible = !PainelDados.IsVisible;

        BotaoDados.Text = PainelDados.IsVisible
            ? "Dados cadastrais  ▴"
            : "Dados cadastrais  ▾";
    }

    private async void OnTrocarFotoClicked(object sender, EventArgs e)
    {
        if (!_proprio || _membro == null || _trocandoFoto)
            return;

        string? idAuth = _auth.UsuarioLogado?.IdAuth;

        if (string.IsNullOrEmpty(idAuth))
            return;

        _trocandoFoto = true;
        BotaoFoto.IsEnabled = false;

        try
        {
            FileResult? foto = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Escolha a sua foto",
                FileTypes = FilePickerFileType.Images
            });

            if (foto == null)
                return;

            await _fotos.TrocarAsync(_membro, foto, idAuth);

            await CarregarFotoAsync();
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Foto", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert(
                "Foto",
                "Não foi possível enviar a foto. Verifique a internet e tente novamente.",
                "OK");
        }
        finally
        {
            _trocandoFoto = false;
            BotaoFoto.IsEnabled = true;
        }
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (_membro == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(EditarMembroPage),
            new ShellNavigationQueryParameters
            {
                { "Membro", _membro }
            });
    }
}