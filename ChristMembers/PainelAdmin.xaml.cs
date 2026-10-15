using ChristMembers.Services;

namespace ChristMembers;

public partial class PainelAdmin : ContentPage
{
    private readonly PedidoOracaoService _pedidoService;

    public PainelAdmin(PedidoOracaoService pedidoService)
    {
        InitializeComponent();

        _pedidoService = pedidoService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await AtualizarContador();
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
}