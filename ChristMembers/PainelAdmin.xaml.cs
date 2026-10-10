namespace ChristMembers;

public partial class PainelAdmin : ContentPage
{
    public PainelAdmin()
    {
        InitializeComponent();
    }

    private async void OnMembrosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ListaMembros));
    }

    private async void OnVisitantesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ListaVisitantes));
    }

    private async void OnNovidadesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovidadesPage));
    }
}