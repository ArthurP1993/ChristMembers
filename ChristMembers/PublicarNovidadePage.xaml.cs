using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class PublicarNovidadePage : ContentPage
{
    public PublicarNovidadePage(PublicarNovidadeViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}