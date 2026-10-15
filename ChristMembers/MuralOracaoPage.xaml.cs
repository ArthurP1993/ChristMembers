using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class MuralOracaoPage : ContentPage
{
    private readonly MuralOracaoViewModel _viewModel;

    public MuralOracaoPage(MuralOracaoViewModel viewModel)
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