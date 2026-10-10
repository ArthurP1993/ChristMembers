using System.Collections.ObjectModel;
using ChristMembers.Data;
using ChristMembers.Models;

namespace ChristMembers.Services;

public class NovidadeService
{
    private readonly BancoDados _bancoDados;

    public ObservableCollection<Novidade> Novidades { get; } =
        new ObservableCollection<Novidade>();

    public NovidadeService(BancoDados bancoDados)
    {
        _bancoDados = bancoDados;
    }

    public async Task Inicializar()
    {
        List<Novidade> todas = await _bancoDados.ListarNovidades();

        foreach (Novidade vencida in todas.Where(n => n.Expirada).ToList())
        {
            await _bancoDados.ExcluirNovidade(vencida);

            ApagarFoto(vencida);
        }

        Novidades.Clear();

        foreach (Novidade novidade in Ordenar(todas.Where(n => !n.Expirada)))
        {
            Novidades.Add(novidade);
        }
    }

    public async Task Publicar(
        string titulo,
        string descricao,
        DateTime? dataEvento,
        FileResult? foto)
    {
        string nomeArquivo = string.Empty;

        if (foto != null)
            nomeArquivo = await SalvarFoto(foto);

        Novidade novidade = new Novidade
        {
            Titulo = titulo.Trim(),
            Descricao = descricao.Trim(),
            NomeArquivoFoto = nomeArquivo,
            DataPublicacao = DateTime.Now,
            DataEvento = dataEvento?.Date
        };

        await _bancoDados.SalvarNovidade(novidade);

        List<Novidade> ordenadas = Ordenar(Novidades.Append(novidade)).ToList();

        Novidades.Insert(ordenadas.IndexOf(novidade), novidade);
    }

    public async Task Excluir(Novidade novidade)
    {
        await _bancoDados.ExcluirNovidade(novidade);

        Novidades.Remove(novidade);

        ApagarFoto(novidade);
    }

    private static async Task<string> SalvarFoto(FileResult foto)
    {
        string pasta = Path.Combine(FileSystem.AppDataDirectory, "novidades");

        Directory.CreateDirectory(pasta);

        string extensao = Path.GetExtension(foto.FileName);

        if (string.IsNullOrEmpty(extensao))
            extensao = ".jpg";

        string nomeArquivo = $"{Guid.NewGuid():N}{extensao}";

        using Stream origem = await foto.OpenReadAsync();
        using FileStream destino = File.Create(Path.Combine(pasta, nomeArquivo));

        await origem.CopyToAsync(destino);

        return nomeArquivo;
    }

    private static void ApagarFoto(Novidade novidade)
    {
        if (!novidade.TemFoto)
            return;

        try
        {
            if (File.Exists(novidade.CaminhoFoto))
                File.Delete(novidade.CaminhoFoto);
        }
        catch
        {
            // Se não conseguir apagar o arquivo, a novidade já saiu da lista e do banco.
        }
    }

    private static IEnumerable<Novidade> Ordenar(IEnumerable<Novidade> novidades)
    {
        return novidades
            .OrderBy(n => !n.DataEvento.HasValue)
            .ThenBy(n => n.DataEvento ?? DateTime.MaxValue)
            .ThenByDescending(n => n.Id);
    }

}