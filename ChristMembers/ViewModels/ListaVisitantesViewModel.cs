using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ListaVisitantesViewModel : INotifyPropertyChanged
{
    private readonly VisitanteService _visitanteService;
    private readonly AuthService _auth;

    private string _textoBusca = string.Empty;
    private string _statusFiltro = "Todos";
    private string _resumo = string.Empty;

    public ObservableCollection<Visitante> Visitantes { get; } = new();

    public List<string> OpcoesStatus { get; } = new()
    {
        "Todos",
        "Novos",
        "Contatados",
        "Voltaram"
    };

    public bool EhAdmin => _auth.UsuarioLogado?.Cargo == AuthService.CargoAdmin;

    public string TextoBusca
    {
        get => _textoBusca;

        set
        {
            if (SetProperty(ref _textoBusca, value ?? string.Empty))
                AplicarFiltro();
        }
    }

    public string StatusFiltro
    {
        get => _statusFiltro;

        set
        {
            if (SetProperty(ref _statusFiltro, value ?? "Todos"))
                AplicarFiltro();
        }
    }

    public string Resumo
    {
        get => _resumo;
        private set => SetProperty(ref _resumo, value);
    }

    public ICommand AlterarStatusCommand { get; }
    public ICommand SalvarAnotacaoCommand { get; }
    public ICommand WhatsAppCommand { get; }
    public ICommand ExcluirCommand { get; }

    public ListaVisitantesViewModel(VisitanteService visitanteService, AuthService auth)
    {
        _visitanteService = visitanteService;
        _auth = auth;

        AlterarStatusCommand = new Command<Visitante>(async v => await Executar(() => AlterarStatus(v)));
        SalvarAnotacaoCommand = new Command<Visitante>(async v => await Executar(() => SalvarAnotacao(v)));
        ExcluirCommand = new Command<Visitante>(async v => await Executar(() => Excluir(v)));
        WhatsAppCommand = new Command<Visitante>(async v => await AbrirWhatsApp(v));
    }

    public async Task Inicializar()
    {
        if (!EhAdmin)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        try
        {
            await _visitanteService.Inicializar();
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert(
                "Sem conexão",
                "Não foi possível carregar os visitantes. Verifique a internet.",
                "OK");
        }

        AplicarFiltro();
    }

    private bool Combina(Visitante visitante)
    {
        bool combinaStatus = StatusFiltro switch
        {
            "Novos" => visitante.Status == StatusVisitante.Novo,
            "Contatados" => visitante.Status == StatusVisitante.Contatado,
            "Voltaram" => visitante.Status == StatusVisitante.Voltou,
            _ => true
        };

        return combinaStatus
            && BuscaHelper.Combina(TextoBusca, visitante.Nome, visitante.Telefone);
    }

    private void AplicarFiltro()
    {
        Visitantes.Clear();

        foreach (Visitante visitante in _visitanteService.Visitantes.Where(Combina))
        {
            Visitantes.Add(visitante);
        }

        AtualizarResumo();
    }

    private void AtualizarResumo()
    {
        int total = _visitanteService.Visitantes.Count;
        int novos = _visitanteService.Visitantes.Count(v => v.Status == StatusVisitante.Novo);

        bool filtrando = !string.IsNullOrWhiteSpace(TextoBusca) || StatusFiltro != "Todos";

        Resumo = filtrando
            ? $"Exibindo {Visitantes.Count} de {total}"
            : $"Total: {total} · {novos} novo(s)";
    }

    private void RefletirNaTela(Visitante visitante)
    {
        int indice = Visitantes.IndexOf(visitante);

        if (indice >= 0)
        {
            if (Combina(visitante))
                Visitantes[indice] = visitante;
            else
                Visitantes.RemoveAt(indice);
        }

        AtualizarResumo();
    }

    private async Task AlterarStatus(Visitante? visitante)
    {
        if (visitante == null || !EhAdmin)
            return;

        await _visitanteService.AlterarStatus(visitante, visitante.ProximoStatus);

        RefletirNaTela(visitante);
    }

    private async Task SalvarAnotacao(Visitante? visitante)
    {
        if (visitante == null || !EhAdmin)
            return;

        await _visitanteService.SalvarAnotacao(visitante, visitante.Anotacao ?? string.Empty);

        RefletirNaTela(visitante);

        await Shell.Current.DisplayAlert("Pronto", "Anotação salva.", "OK");
    }

    private async Task Excluir(Visitante? visitante)
    {
        if (visitante == null || !EhAdmin)
            return;

        bool confirmou = await Shell.Current.DisplayAlert(
            "Excluir visitante",
            $"Deseja excluir \"{visitante.Nome}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmou)
            return;

        await _visitanteService.Inativar(visitante);

        Visitantes.Remove(visitante);

        AtualizarResumo();
    }

    private async Task AbrirWhatsApp(Visitante? visitante)
    {
        if (visitante == null || !EhAdmin || !visitante.PodeWhatsApp)
            return;

        string numero = CpfValidador.SomenteDigitos(visitante.Telefone);

        if (numero.Length is 10 or 11)
            numero = "55" + numero;

        string texto =
            $"Olá, {visitante.PrimeiroNome}! 🙏 " +
            "Foi uma alegria receber você na nossa igreja. " +
            "Você é sempre bem-vindo(a)! Se precisar de algo, conte com a gente.";

        try
        {
            await Launcher.Default.OpenAsync(
                new Uri($"https://wa.me/{numero}?text={Uri.EscapeDataString(texto)}"));
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Aviso", "Não foi possível abrir o WhatsApp.", "OK");
        }
    }

    private static async Task Executar(Func<Task> acao)
    {
        try
        {
            await acao();
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                "Não foi possível concluir. Verifique a internet e tente novamente.",
                "OK");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propriedade = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propriedade));
    }

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? propriedade = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor))
            return false;

        campo = valor;
        OnPropertyChanged(propriedade);
        return true;
    }
}