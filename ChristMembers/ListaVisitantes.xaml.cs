using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class ListaVisitantes : ContentPage
{
    private readonly ListaVisitantesViewModel _viewModel;

    public ListaVisitantes(ListaVisitantesViewModel viewModel)
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