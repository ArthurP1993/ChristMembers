using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class ListaMembros : ContentPage
{
    private readonly ListaMembrosViewModel _viewModel;

    public ListaMembros(ListaMembrosViewModel viewModel)
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