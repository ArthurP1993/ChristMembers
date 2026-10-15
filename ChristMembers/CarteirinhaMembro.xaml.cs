using ChristMembers.Models;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class CarteirinhaMembro : ContentPage, IQueryAttributable
{
    private Membro? _membro;

    public CarteirinhaMembro()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Membro", out object? valor) && valor is Membro membro)
        {
            _membro = membro;

            BindingContext = new CarteirinhaViewModel(membro);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_membro != null)
            BindingContext = new CarteirinhaViewModel(_membro);
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (_membro == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(EditarMembroPage),
            new ShellNavigationQueryParameters
            {
                { "Membro", _membro }
            });
    }
}