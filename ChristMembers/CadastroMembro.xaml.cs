using ChristMembers.Services;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class CadastroMembro : ContentPage
{
    private readonly AuthService _auth;

    public CadastroMembro(MembrosViewModel viewModel, AuthService auth)
    {
        InitializeComponent();

        _auth = auth;

        BindingContext = viewModel;

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Sair",
            Command = new Command(async () => await SairAsync())
        });
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