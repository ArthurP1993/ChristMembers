namespace ChristMembers;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void ClearClicked(object sender, EventArgs e)
    {
        Usuario.Text = string.Empty;
        Senha.Text = string.Empty;
    }
}