using Microsoft.Maui.Graphics.Platform;
using MauiFormato = Microsoft.Maui.Graphics.ImageFormat;
using MauiImagem = Microsoft.Maui.Graphics.IImage;

namespace ChristMembers.Helpers;

public static class FotoHelper
{
    // Devolve a foto reduzida em JPEG, ou null se não foi possível reduzir.
    public static byte[]? Reduzir(byte[] original, float ladoMaximo = 1600f, float qualidade = 0.85f)
    {
        try
        {
            using MemoryStream entrada = new(original);

            MauiImagem? imagem = PlatformImage.FromStream(entrada);

            if (imagem is null)
                return null;

            MauiImagem reduzida = imagem.Downsize(ladoMaximo, true);

            byte[] resultado = reduzida.AsBytes(MauiFormato.Jpeg, qualidade);

            return resultado.Length > 0 ? resultado : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}