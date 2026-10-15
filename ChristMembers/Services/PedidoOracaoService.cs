using System.Collections.ObjectModel;
using ChristMembers.Data;
using ChristMembers.Models;

namespace ChristMembers.Services;

public class PedidoOracaoService
{
    private readonly BancoDados _bancoDados;

    public ObservableCollection<PedidoOracao> Pedidos { get; } =
        new ObservableCollection<PedidoOracao>();

    public PedidoOracaoService(BancoDados bancoDados)
    {
        _bancoDados = bancoDados;
    }

    public async Task Inicializar()
    {
        List<PedidoOracao> pedidos = await _bancoDados.ListarPedidosOracao();

        Pedidos.Clear();

        foreach (PedidoOracao pedido in pedidos)
        {
            Pedidos.Add(pedido);
        }
    }

    public async Task Enviar(
        string nome,
        bool anonimo,
        string telefone,
        string categoria,
        string texto,
        bool permiteMural)
    {
        PedidoOracao pedido = new PedidoOracao
        {
            Nome = anonimo ? string.Empty : nome.Trim(),
            Anonimo = anonimo,
            Telefone = anonimo ? string.Empty : telefone.Trim(),
            Categoria = categoria,
            Texto = texto.Trim(),
            PermiteMural = permiteMural,
            Status = StatusPedido.Novo,
            DataPedido = DateTime.Now
        };

        await _bancoDados.SalvarPedidoOracao(pedido);
    }

    public async Task AlterarStatus(PedidoOracao pedido, StatusPedido status)
    {
        pedido.Status = status;

        await _bancoDados.AtualizarPedidoOracao(pedido);

        AtualizarNaLista(pedido);
    }

    public async Task SalvarAnotacao(PedidoOracao pedido, string anotacao)
    {
        pedido.Anotacao = anotacao.Trim();

        await _bancoDados.AtualizarPedidoOracao(pedido);

        AtualizarNaLista(pedido);
    }

    public async Task Arquivar(PedidoOracao pedido)
    {
        pedido.Arquivado = true;

        await _bancoDados.AtualizarPedidoOracao(pedido);

        Pedidos.Remove(pedido);
    }

    public async Task<int> ContarNovos()
    {
        return await _bancoDados.ContarPedidosNovos();
    }

    private void AtualizarNaLista(PedidoOracao pedido)
    {
        int indice = Pedidos.IndexOf(pedido);

        if (indice >= 0)
            Pedidos[indice] = pedido;
    }

    public async Task<List<PedidoOracao>> ListarMural()
    {
        return await _bancoDados.ListarPedidosMural();
    }

    public async Task<int> RegistrarOracao(int id)
    {
        return await _bancoDados.IncrementarOracoes(id);
    }

    public async Task AlternarMural(PedidoOracao pedido)
    {
        pedido.AprovadoMural = !pedido.AprovadoMural;

        await _bancoDados.AtualizarPedidoOracao(pedido);

        AtualizarNaLista(pedido);
    }
}