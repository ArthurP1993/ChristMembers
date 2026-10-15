using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class ListaPedidosOracaoPage : ContentPage
{
    private readonly ListaPedidosOracaoViewModel _viewModel;

    public ListaPedidosOracaoPage(ListaPedidosOracaoViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.Inicializar();
    }
}