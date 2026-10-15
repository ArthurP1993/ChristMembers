using ChristMembers.ViewModels;
using ChristMembers.Models;

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

    private async void OnMembroSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Membro membro)
            return;

        ((CollectionView)sender).SelectedItem = null;

        await Shell.Current.GoToAsync(
            nameof(CarteirinhaMembro),
            new ShellNavigationQueryParameters
            {
            { "Membro", membro }
            });
    }
}