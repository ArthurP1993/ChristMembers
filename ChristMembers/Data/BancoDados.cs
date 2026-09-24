using SQLite;
using ChristMembers.Models;

namespace ChristMembers.Data;

public class BancoDados
{
    private SQLiteAsyncConnection _banco;

    private async Task Inicializar()
    {
        if (_banco != null)
            return;

        string caminhoBanco = Path.Combine(
            FileSystem.AppDataDirectory,
            "ChristMembers.db3");

        _banco = new SQLiteAsyncConnection(caminhoBanco);

        await _banco.CreateTableAsync<Membro>();
    }

    public async Task<List<Membro>> ListarMembros()
    {
        await Inicializar();

        return await _banco.Table<Membro>().ToListAsync();
    }

    public async Task<int> SalvarMembro(Membro membro)
    {
        await Inicializar();

        return await _banco.InsertAsync(membro);
    }

    public async Task<int> ExcluirMembro(Membro membro)
    {
        await Inicializar();

        return await _banco.DeleteAsync(membro);
    }
}