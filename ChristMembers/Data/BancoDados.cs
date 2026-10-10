using SQLite;
using ChristMembers.Models;

namespace ChristMembers.Data;

public class BancoDados
{
    private SQLiteAsyncConnection _banco;

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

            SQLiteAsyncConnection conexao = new SQLiteAsyncConnection(caminhoBanco);

            await conexao.CreateTableAsync<Membro>();
            await conexao.CreateTableAsync<Visitante>();
            await conexao.CreateTableAsync<Usuario>();
            await conexao.CreateTableAsync<Novidade>();

            _banco = conexao;
        }
        finally
        {
            _trava.Release();
        }
    }

    public async Task<List<Membro>> ListarMembros()
    {
        await Inicializar();

        return await _banco.Table<Membro>().ToListAsync();
    }

    public async Task<int> SalvarMembro(Membro membro)
    {
        await Inicializar();

        if (membro.Id == 0)
            return await _banco.InsertAsync(membro);

        return await _banco.UpdateAsync(membro);
    }

    public async Task<int> ExcluirMembro(Membro membro)
    {
        await Inicializar();

        return await _banco.DeleteAsync(membro);
    }

    public async Task<List<Visitante>> ListarVisitantes()
    {
        await Inicializar();

        return await _banco!.Table<Visitante>().ToListAsync();
    }

    public async Task<int> SalvarVisitante(Visitante visitante)
    {
        await Inicializar();

        return await _banco!.InsertAsync(visitante);
    }

    public async Task<int> ExcluirVisitante(Visitante visitante)
    {
        await Inicializar();

        return await _banco!.DeleteAsync(visitante);
    }

    public async Task<bool> ExisteAlgumUsuario()
    {
        await Inicializar();

        return await _banco!.Table<Usuario>().CountAsync() > 0;
    }

    public async Task<Usuario?> BuscarUsuario(string nomeUsuario)
    {
        await Inicializar();

        return await _banco!.Table<Usuario>()
            .Where(u => u.NomeUsuario == nomeUsuario)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarUsuario(Usuario usuario)
    {
        await Inicializar();

        return await _banco!.InsertAsync(usuario);
    }

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
}