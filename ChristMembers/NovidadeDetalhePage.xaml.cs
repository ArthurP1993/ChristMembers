using ChristMembers.Models;
using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class NovidadeDetalhePage : ContentPage, IQueryAttributable
{
    private readonly NovidadeDetalheViewModel _viewModel;

    public NovidadeDetalhePage(NovidadeDetalheViewModel viewModel)
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

        await _viewModel.AtualizarAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        await _viewModel.EncerrarLoginAsync();
    }
}