using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class PresencasPage : ContentPage
{
    private readonly PresencasViewModel _viewModel;

    public PresencasPage(PresencasViewModel viewModel)
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

    private async void OnEventoSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not ResumoEvento item)
            return;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync(
            nameof(ConfirmadosPage),
            new ShellNavigationQueryParameters
            {
                { "Novidade", item.Novidade }
            });
    }
}