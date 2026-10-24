using ChristMembers.Models;
using ChristMembers.Services;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class CarteirinhaMembro : ContentPage, IQueryAttributable
{
    private readonly AuthService _auth;

    private Membro? _membro;
    private bool _proprio;

    public CarteirinhaMembro(AuthService auth)
    {
        InitializeComponent();

        _auth = auth;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_membro != null)
            BindingContext = new CarteirinhaViewModel(_membro);
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

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Sair",
            Command = new Command(async () => await SairAsync())
        });
    }

    private async Task SairAsync()
    {
        await _auth.SairAsync();

        await Shell.Current.GoToAsync("..");
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