using System.Collections.ObjectModel;
using ChristMembers.Helpers;
using ChristMembers.Models;
using Supabase;

namespace ChristMembers.Services;

public class NovidadeService
{
    private const string Pasta = "novidades";
    private const int LimiteBytes = 5 * 1024 * 1024;

    private readonly SupabaseServico _supabase;
    private readonly AuthService _auth;

    public ObservableCollection<Novidade> Novidades { get; } = new();

    public NovidadeService(SupabaseServico supabase, AuthService auth)
    {
        _supabase = supabase;
        _auth = auth;
    }

    private static IEnumerable<Novidade> Ordenar(IEnumerable<Novidade> novidades)
    {
        return novidades
            .OrderBy(n => !n.DataEvento.HasValue)
            .ThenBy(n => n.DataEvento ?? DateTime.MaxValue)
            .ThenByDescending(n => n.Id);
    }

    public async Task Inicializar()
    {
        Client cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<Novidade>().Get();

        List<Novidade> todas = resposta.Models;

        // Só o admin pode apagar. As vencidas já ficam invisíveis para os visitantes.
        if (_auth.UsuarioLogado?.Cargo == AuthService.CargoAdmin)
        {
            foreach (Novidade vencida in todas.Where(n => n.Expirada).ToList())
            {
                try
                {
                    await ExcluirNoServidor(cliente, vencida);
                }
                catch (Exception)
                {
                    // Se não conseguir agora, tenta de novo na próxima abertura.
                }
            }
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
        Client cliente = await _supabase.ObterAsync();

        string nomeArquivo = string.Empty;

        if (foto != null)
            nomeArquivo = await EnviarFoto(cliente, foto);

        Novidade novidade = new()
        {
            Titulo = titulo.Trim(),
            Descricao = descricao.Trim(),
            NomeArquivoFoto = nomeArquivo,
            DataEvento = dataEvento?.Date
        };

        try
        {
            await cliente.From<Novidade>().Insert(novidade);
        }
        catch
        {
            await ApagarFoto(cliente, nomeArquivo);
            throw;
        }
    }

    public async Task Excluir(Novidade novidade)
    {
        Client cliente = await _supabase.ObterAsync();

        await ExcluirNoServidor(cliente, novidade);

        Novidades.Remove(novidade);
    }

    private static async Task ExcluirNoServidor(Client cliente, Novidade novidade)
    {
        await cliente.From<Novidade>().Delete(novidade);

        await ApagarFoto(cliente, novidade.NomeArquivoFoto);
    }

    private static async Task<string> EnviarFoto(Client cliente, FileResult foto)
    {
        string extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extensao))
            extensao = ".jpg";

        if (extensao is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            throw new InvalidOperationException("Formato de imagem não aceito. Use JPG, PNG ou WebP.");

        byte[] original;

        using (Stream origem = await foto.OpenReadAsync())
        using (MemoryStream memoria = new())
        {
            await origem.CopyToAsync(memoria);
            original = memoria.ToArray();
        }

        byte[] bytes = original;

        byte[]? reduzida = FotoHelper.Reduzir(original);

        if (reduzida != null && reduzida.Length < original.Length)
        {
            bytes = reduzida;
            extensao = ".jpg";
        }

        if (bytes.Length > LimiteBytes)
            throw new InvalidOperationException("A foto passa de 5 MB. Escolha uma menor.");

        string nomeArquivo = $"{Guid.NewGuid():N}{extensao}";

        await cliente.Storage.From(Pasta).Upload(bytes, nomeArquivo);

        return nomeArquivo;
    }

    private static async Task ApagarFoto(Client cliente, string nomeArquivo)
    {
        if (string.IsNullOrEmpty(nomeArquivo))
            return;

        try
        {
            await cliente.Storage.From(Pasta).Remove(nomeArquivo);
        }
        catch (Exception)
        {
            // A novidade já saiu do banco; uma foto sobrando não quebra nada.
        }
    }
}