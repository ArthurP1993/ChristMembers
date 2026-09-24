using System.Collections.ObjectModel;
using ChristMembers.Data;
using ChristMembers.Models;

namespace ChristMembers.Services;

public class MembroService
{
    private readonly BancoDados _bancoDados;

    public ObservableCollection<Membro> Membros { get; } =
        new ObservableCollection<Membro>();

    public MembroService(BancoDados bancoDados)
    {
        _bancoDados = bancoDados;
    }

    public async Task Inicializar()
    {
        List<Membro> membros = await _bancoDados.ListarMembros();

        Membros.Clear();

        foreach (Membro membro in membros)
        {
            Membros.Add(membro);
        }
    }

    public async Task Cadastrar(Membro membro)
    {
        await _bancoDados.SalvarMembro(membro);

        Membros.Add(membro);
    }

    public async Task Excluir(Membro membro)
    {
        await _bancoDados.ExcluirMembro(membro);

        Membros.Remove(membro);
    }
}