using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class AniversariantesPage : ContentPage
{
    private readonly AniversariantesViewModel _viewModel;

    public AniversariantesPage(AniversariantesViewModel viewModel)
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