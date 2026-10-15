using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class NovoPedidoOracaoPage : ContentPage
{
    public NovoPedidoOracaoPage(NovoPedidoOracaoViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    private async void OnMuralClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MuralOracaoPage));
    }
}