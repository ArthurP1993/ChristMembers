using SQLite;
using ChristMembers.Models;

namespace ChristMembers.Data;

public class BancoDados
{
    private SQLiteAsyncConnection? _banco;

    private readonly SemaphoreSlim _trava = new(1, 1);

    private async Task Inicializar()
    {
        if (_banco != null)
            return;

        await _trava.WaitAsync();

        try
        {
            if (_banco != null)
                return;

            string caminhoBanco = Path.Combine(
                FileSystem.AppDataDirectory,
                "ChristMembers.db3");

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"BANCO: {caminhoBanco}");
#endif

            SQLiteAsyncConnection conexao = new SQLiteAsyncConnection(caminhoBanco);

            await conexao.CreateTableAsync<Membro>();
            await conexao.CreateTableAsync<Novidade>();
            await conexao.CreateTableAsync<PedidoOracao>();

            _banco = conexao;
        }
        finally
        {
            _trava.Release();
        }
    }

    // ---------- Membros ----------

    public async Task<List<Membro>> ListarMembros()
    {
        await Inicializar();

        List<Membro> todos = await _banco!.Table<Membro>().ToListAsync();

        return todos.Where(m => !m.Inativo).ToList();
    }

    public async Task<List<Membro>> ListarMembrosInativos()
    {
        await Inicializar();

        List<Membro> todos = await _banco!.Table<Membro>().ToListAsync();

        return todos
            .Where(m => m.Inativo)
            .OrderBy(m => m.Nome)
            .ToList();
    }

    public async Task<int> SalvarMembro(Membro membro)
    {
        await Inicializar();

        return await _banco!.InsertAsync(membro);
    }

    public async Task<int> AtualizarMembro(Membro membro)
    {
        await Inicializar();

        return await _banco!.UpdateAsync(membro);
    }

    public async Task<int> ExcluirMembro(Membro membro)
    {
        await Inicializar();

        return await _banco!.DeleteAsync(membro);
    }

    // ---------- Novidades ----------

    public async Task<List<Novidade>> ListarNovidades()
    {
        await Inicializar();

        return await _banco!.Table<Novidade>()
            .OrderByDescending(n => n.Id)
            .ToListAsync();
    }

    public async Task<int> SalvarNovidade(Novidade novidade)
    {
        await Inicializar();

        return await _banco!.InsertAsync(novidade);
    }

    public async Task<int> ExcluirNovidade(Novidade novidade)
    {
        await Inicializar();

        return await _banco!.DeleteAsync(novidade);
    }

    // ---------- Pedidos de oração ----------

    public async Task<List<PedidoOracao>> ListarPedidosOracao()
    {
        await Inicializar();

        List<PedidoOracao> todos = await _banco!.Table<PedidoOracao>().ToListAsync();

        return todos
            .Where(p => !p.Arquivado)
            .OrderByDescending(p => p.Id)
            .ToList();
    }

    public async Task<int> SalvarPedidoOracao(PedidoOracao pedido)
    {
        await Inicializar();

        return await _banco!.InsertAsync(pedido);
    }

    public async Task<int> AtualizarPedidoOracao(PedidoOracao pedido)
    {
        await Inicializar();

        return await _banco!.UpdateAsync(pedido);
    }

    public async Task<int> ContarPedidosNovos()
    {
        List<PedidoOracao> pedidos = await ListarPedidosOracao();

        return pedidos.Count(p => p.Status == StatusPedido.Novo);
    }

    public async Task<List<PedidoOracao>> ListarPedidosMural()
    {
        List<PedidoOracao> todos = await ListarPedidosOracao();

        DateTime limite = DateTime.Today.AddDays(-30);

        return todos
            .Where(p => p.PermiteMural && p.AprovadoMural && p.DataPedido.Date >= limite)
            .ToList();
    }

    public async Task<int> IncrementarOracoes(int id)
    {
        await Inicializar();

        PedidoOracao? pedido = await _banco!.FindAsync<PedidoOracao>(id);

        if (pedido == null)
            return 0;

        pedido.Oracoes++;

        await _banco.UpdateAsync(pedido);

        return pedido.Oracoes;
    }
}