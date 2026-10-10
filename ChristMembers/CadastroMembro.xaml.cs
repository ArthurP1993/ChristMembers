using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class CadastroMembro : ContentPage
{
    private readonly MembrosViewModel _viewModel;

    public CadastroMembro(MembrosViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }
}