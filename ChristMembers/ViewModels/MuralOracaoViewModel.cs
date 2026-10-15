using System.Collections.ObjectModel;
using System.Windows.Input;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class MuralOracaoViewModel
{
    private const string ChavePreferencia = "oracoes_feitas";

    private readonly PedidoOracaoService _pedidoService;

    public ObservableCollection<PedidoOracao> Pedidos { get; } = new();

    public ICommand OrarCommand { get; }

    public MuralOracaoViewModel(PedidoOracaoService pedidoService)
    {
        _pedidoService = pedidoService;

        OrarCommand = new Command<PedidoOracao>(async p => await Orar(p));
    }

    public async Task Inicializar()
    {
        HashSet<int> feitas = LerOracoesFeitas();

        List<PedidoOracao> mural = await _pedidoService.ListarMural();

        Pedidos.Clear();

        foreach (PedidoOracao pedido in mural)
        {
            pedido.JaOrei = feitas.Contains(pedido.Id);

            Pedidos.Add(pedido);
        }
    }

    private async Task Orar(PedidoOracao? pedido)
    {
        if (pedido == null || pedido.JaOrei)
            return;

        int total = await _pedidoService.RegistrarOracao(pedido.Id);

        if (total == 0)
        {
            Pedidos.Remove(pedido);
            return;
        }

        pedido.Oracoes = total;
        pedido.JaOrei = true;

        SalvarOracaoFeita(pedido.Id);

        int indice = Pedidos.IndexOf(pedido);

        if (indice >= 0)
            Pedidos[indice] = pedido;
    }

    private static HashSet<int> LerOracoesFeitas()
    {
        string texto = Preferences.Default.Get(ChavePreferencia, string.Empty);

        return texto
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => int.TryParse(t, out int id) ? id : 0)
            .Where(id => id > 0)
            .ToHashSet();
    }

    private static void SalvarOracaoFeita(int id)
    {
        HashSet<int> feitas = LerOracoesFeitas();

        feitas.Add(id);

        Preferences.Default.Set(ChavePreferencia, string.Join(",", feitas));
    }
}