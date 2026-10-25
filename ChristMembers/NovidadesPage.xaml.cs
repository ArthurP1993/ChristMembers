using ChristMembers.Models;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class NovidadesPage : ContentPage
{
    private readonly NovidadesViewModel _viewModel;

    public NovidadesPage(NovidadesViewModel viewModel)
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

    private async void OnPublicarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PublicarNovidadePage));
    }

    private async void OnNovidadeSelecionada(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Novidade novidade)
            return;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync(
            nameof(NovidadeDetalhePage),
            new ShellNavigationQueryParameters
            {
                { "Novidade", novidade }
            });
    }
}