using ChristMembers.Helpers;
using ChristMembers.Models;
using Newtonsoft.Json;

namespace ChristMembers.Services;

public class FotoMembroService
{
    private const string Pasta = "fotos-membros";
    private const int LimiteBytes = 2 * 1024 * 1024;

    private readonly SupabaseServico _supabase;

    public FotoMembroService(SupabaseServico supabase)
    {
        _supabase = supabase;
    }

    // Endereço temporário (1 hora): a pasta é privada.
    public async Task<string?> ObterUrlAsync(Membro membro)
    {
        if (string.IsNullOrWhiteSpace(membro.NomeFoto))
            return null;

        var cliente = await _supabase.ObterAsync();

        return await cliente.Storage.From(Pasta).CreateSignedUrl(membro.NomeFoto, 3600);
    }

    // Só o próprio membro troca a foto.
    public async Task TrocarAsync(Membro membro, FileResult foto, string idAuth)
    {
        string extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();

        if (extensao is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            throw new InvalidOperationException("Formato de imagem não aceito. Use JPG, PNG ou WebP.");

        byte[] original;

        using (Stream origem = await foto.OpenReadAsync())
        using (MemoryStream memoria = new())
        {
            await origem.CopyToAsync(memoria);
            original = memoria.ToArray();
        }

        byte[] bytes;

        byte[]? reduzida = FotoHelper.Reduzir(original, 800f, 0.85f);

        if (reduzida != null)
            bytes = reduzida;
        else if (extensao is ".jpg" or ".jpeg")
            bytes = original;
        else
            throw new InvalidOperationException("Não foi possível preparar a foto. Escolha uma imagem JPG.");

        if (bytes.Length > LimiteBytes)
            throw new InvalidOperationException("A foto passa de 2 MB. Escolha uma menor.");

        string caminho = $"{idAuth.ToLowerInvariant()}/{Guid.NewGuid():N}.jpg";

        var cliente = await _supabase.ObterAsync();

        await cliente.Storage.From(Pasta).Upload(bytes, caminho);

        string? antiga;

        try
        {
            var resposta = await cliente.Rpc(
                "definir_foto_membro",
                new Dictionary<string, object> { { "p_nome", caminho } });

            antiga = JsonConvert.DeserializeObject<string?>(resposta.Content ?? "null");
        }
        catch
        {
            await ApagarAsync(caminho);
            throw;
        }

        membro.NomeFoto = caminho;

        if (!string.IsNullOrWhiteSpace(antiga) && antiga != caminho)
            await ApagarAsync(antiga);
    }

    private async Task ApagarAsync(string caminho)
    {
        try
        {
            var cliente = await _supabase.ObterAsync();

            await cliente.Storage.From(Pasta).Remove(caminho);
        }
        catch (Exception)
        {
            // Um arquivo sobrando não quebra nada.
        }
    }
}