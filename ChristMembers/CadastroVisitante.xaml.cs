using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class CadastroVisitante : ContentPage
{
    private readonly VisitantesViewModel _viewModel;

    public CadastroVisitante(VisitantesViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }
}