namespace ChristMembers;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void SignUpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    private async void OnMembrosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    private async void OnVisitantesClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Visitantes",
            "Vamos criar essa página no próximo passo.",
            "OK");
    }
}