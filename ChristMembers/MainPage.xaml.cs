using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class MainPage : ContentPage
{
    private readonly MembrosViewModel _viewModel;

    public MainPage(MembrosViewModel viewModel)
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