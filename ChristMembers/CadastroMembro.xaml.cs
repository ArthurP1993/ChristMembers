using ChristMembers.Services;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class CadastroMembro : ContentPage
{
    private readonly AuthService _auth;
    private readonly MembrosViewModel _viewModel;

    public CadastroMembro(MembrosViewModel viewModel, AuthService auth)
    {
        InitializeComponent();

        _auth = auth;
        _viewModel = viewModel;

        BindingContext = viewModel;

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Sair",
            Command = new Command(async () => await SairAsync())
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CarregarListasAsync();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async Task SairAsync()
    {
        await _auth.SairAsync();

        await Shell.Current.GoToAsync("..");
    }
}