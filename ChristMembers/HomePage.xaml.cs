namespace ChristMembers;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroMembro));
    }

    private async void OnLoginMembrosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    private async void OnNovidadesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovidadesPage));
    }

    private async void OnVisitanteClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroVisitante));
    }

    private async void OnOracaoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovoPedidoOracaoPage));
    }
}