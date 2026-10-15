using ChristMembers.Models;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class EditarMembroPage : ContentPage, IQueryAttributable
{
    private readonly EditarMembroViewModel _viewModel;

    public EditarMembroPage(EditarMembroViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Membro", out object? valor) && valor is Membro membro)
        {
            _viewModel.Carregar(membro);
        }
    }
}