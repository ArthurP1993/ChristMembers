using ChristMembers.ViewModels;

namespace ChristMembers;

public partial class AcompanhantesPage : ContentPage, IQueryAttributable
{
    private readonly AcompanhantesViewModel _viewModel;

    public AcompanhantesPage(AcompanhantesViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        long id = query.TryGetValue("IdConfirmacao", out object? valorId) && valorId is long l ? l : 0;

        string token = query.TryGetValue("Token", out object? valorToken)
            ? valorToken as string ?? string.Empty
            : string.Empty;

        string titulo = query.TryGetValue("Titulo", out object? valorTitulo)
            ? valorTitulo as string ?? string.Empty
            : string.Empty;

        int existentes = query.TryGetValue("Existentes", out object? valorExistentes) && valorExistentes is int e ? e : 0;

        Action<List<string>>? aoConcluir = query.TryGetValue("AoConcluir", out object? valorCallback)
            ? valorCallback as Action<List<string>>
            : null;

        _viewModel.Definir(id, token, titulo, existentes, aoConcluir);
    }

    private void OnRemoverClicked(object sender, EventArgs e)
    {
        if (sender is Button botao && botao.BindingContext is ItemNome item)
            _viewModel.Remover(item);
    }
}