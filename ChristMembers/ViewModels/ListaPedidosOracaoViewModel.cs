using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ListaPedidosOracaoViewModel : INotifyPropertyChanged
{
    private readonly PedidoOracaoService _pedidoService;
    private readonly AuthService _auth;

    private string _statusFiltro = "Todos";
    private string _categoriaFiltro = "Todas";
    private string _resumo = string.Empty;

    public ObservableCollection<PedidoOracao> Pedidos { get; } = new();

    public List<string> OpcoesStatus { get; } = new()
    {
        "Todos",
        "Novos",
        "Em oração",
        "Respondidos"
    };

    public List<string> OpcoesCategoria { get; }

    public bool EhAdmin => _auth.UsuarioLogado?.Cargo == AuthService.CargoAdmin;

    public string StatusFiltro
    {
        get => _statusFiltro;

        set
        {
            if (SetProperty(ref _statusFiltro, value ?? "Todos"))
                AplicarFiltro();
        }
    }

    public string CategoriaFiltro
    {
        get => _categoriaFiltro;

        set
        {
            if (SetProperty(ref _categoriaFiltro, value ?? "Todas"))
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
    public ICommand ArquivarCommand { get; }
    public ICommand LigarCommand { get; }
    public ICommand WhatsAppCommand { get; }
    public ICommand AlternarMuralCommand { get; }

    public ListaPedidosOracaoViewModel(PedidoOracaoService pedidoService, AuthService auth)
    {
        _pedidoService = pedidoService;
        _auth = auth;

        OpcoesCategoria = new List<string> { "Todas" }
            .Concat(CategoriasOracao.Todas)
            .ToList();

        AlterarStatusCommand = new Command<PedidoOracao>(async p => await AlterarStatus(p));
        SalvarAnotacaoCommand = new Command<PedidoOracao>(async p => await SalvarAnotacao(p));
        ArquivarCommand = new Command<PedidoOracao>(async p => await Arquivar(p));
        LigarCommand = new Command<PedidoOracao>(async p => await Ligar(p));
        WhatsAppCommand = new Command<PedidoOracao>(async p => await AbrirWhatsApp(p));
        AlternarMuralCommand = new Command<PedidoOracao>(async p => await AlternarMural(p));
    }

    public async Task Inicializar()
    {
        if (!EhAdmin)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        await _pedidoService.Inicializar();

        AplicarFiltro();
    }

    private bool Combina(PedidoOracao pedido)
    {
        bool combinaStatus = StatusFiltro switch
        {
            "Novos" => pedido.Status == StatusPedido.Novo,
            "Em oração" => pedido.Status == StatusPedido.EmOracao,
            "Respondidos" => pedido.Status == StatusPedido.Respondido,
            _ => true
        };

        bool combinaCategoria =
            CategoriaFiltro == "Todas" || pedido.Categoria == CategoriaFiltro;

        return combinaStatus && combinaCategoria;
    }

    private void AplicarFiltro()
    {
        Pedidos.Clear();

        foreach (PedidoOracao pedido in _pedidoService.Pedidos.Where(Combina))
        {
            Pedidos.Add(pedido);
        }

        AtualizarResumo();
    }

    private void AtualizarResumo()
    {
        int total = _pedidoService.Pedidos.Count;
        int novos = _pedidoService.Pedidos.Count(p => p.Status == StatusPedido.Novo);

        Resumo = $"Exibindo {Pedidos.Count} de {total} · {novos} novo(s)";
    }

    private void RefletirNaTela(PedidoOracao pedido)
    {
        int indice = Pedidos.IndexOf(pedido);

        if (indice >= 0)
        {
            if (Combina(pedido))
                Pedidos[indice] = pedido;
            else
                Pedidos.RemoveAt(indice);
        }

        AtualizarResumo();
    }

    private async Task AlterarStatus(PedidoOracao? pedido)
    {
        if (pedido == null || !EhAdmin)
            return;

        await _pedidoService.AlterarStatus(pedido, pedido.ProximoStatus);

        RefletirNaTela(pedido);
    }

    private async Task SalvarAnotacao(PedidoOracao? pedido)
    {
        if (pedido == null || !EhAdmin)
            return;

        await _pedidoService.SalvarAnotacao(pedido, pedido.Anotacao ?? string.Empty);

        RefletirNaTela(pedido);

        await Shell.Current.DisplayAlert("Pronto", "Anotação salva.", "OK");
    }

    private async Task Arquivar(PedidoOracao? pedido)
    {
        if (pedido == null || !EhAdmin)
            return;

        bool confirmou = await Shell.Current.DisplayAlert(
            "Arquivar pedido",
            "O pedido sai da lista, mas continua guardado.",
            "Arquivar",
            "Cancelar");

        if (!confirmou)
            return;

        await _pedidoService.Arquivar(pedido);

        Pedidos.Remove(pedido);

        AtualizarResumo();
    }

    private async Task Ligar(PedidoOracao? pedido)
    {
        if (pedido == null || !EhAdmin || !pedido.TemTelefone)
            return;

        string digitos = CpfValidador.SomenteDigitos(pedido.Telefone);

        await AbrirLink($"tel:{digitos}", "Não foi possível abrir o discador.");
    }

    private async Task AbrirWhatsApp(PedidoOracao? pedido)
    {
        if (pedido == null || !EhAdmin || !pedido.TemTelefone)
            return;

        string digitos = CpfValidador.SomenteDigitos(pedido.Telefone);

        if (digitos.Length is 10 or 11)
            digitos = "55" + digitos;

        await AbrirLink($"https://wa.me/{digitos}", "Não foi possível abrir o WhatsApp.");
    }

    private static async Task AbrirLink(string endereco, string mensagemErro)
    {
        try
        {
            await Launcher.Default.OpenAsync(new Uri(endereco));
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Aviso", mensagemErro, "OK");
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

    private async Task AlternarMural(PedidoOracao? pedido)
    {
        if (pedido == null || !EhAdmin || !pedido.PermiteMural)
            return;

        await _pedidoService.AlternarMural(pedido);

        RefletirNaTela(pedido);
    }
}