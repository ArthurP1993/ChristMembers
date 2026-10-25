using ChristMembers.Models;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class ConfirmadosPage : ContentPage, IQueryAttributable
{
    private readonly ConfirmadosViewModel _viewModel;

    public ConfirmadosPage(ConfirmadosViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Novidade", out object? valor) && valor is Novidade novidade)
        {
            _viewModel.Definir(novidade);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.Inicializar();
    }
}