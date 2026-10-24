using ChristMembers.Services;

namespace ChristMembers;

public partial class PainelAdmin : ContentPage
{
    private readonly PedidoOracaoService _pedidoService;
    private readonly AuthService _auth;

    public PainelAdmin(PedidoOracaoService pedidoService, AuthService auth)
    {
        InitializeComponent();

        _pedidoService = pedidoService;
        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await AtualizarContador();
    }

    // Bloqueia o botão "voltar" do celular: só o botão Sair leva para a Home.
    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void OnSairClicked(object sender, EventArgs e)
    {
        await _auth.SairAsync();

        await Shell.Current.GoToAsync("..");
    }

    private async Task AtualizarContador()
    {
        try
        {
            int novos = await _pedidoService.ContarNovos();

            BadgePedidos.IsVisible = novos > 0;
            TextoBadgePedidos.Text = novos > 99 ? "99+" : novos.ToString();
        }
        catch (Exception)
        {
            BadgePedidos.IsVisible = false;
        }
    }

    private async void OnMembrosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ListaMembros));
    }

    private async void OnVisitantesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ListaVisitantes));
    }

    private async void OnNovidadesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovidadesPage));
    }

    private async void OnPedidosOracaoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ListaPedidosOracaoPage));
    }
    private async void OnAniversariantesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AniversariantesPage));
    }
}