using System.Globalization;
using System.Text;

namespace ChristMembers.Helpers;

public static class BuscaHelper
{
    public static string Normalizar(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        string decomposto = texto
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        StringBuilder resultado = new();

        foreach (char c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                resultado.Append(c);
        }

        return resultado.ToString().Normalize(NormalizationForm.FormC);
    }

    public static bool Combina(string? termo, string? nome, string? telefone)
    {
        string busca = Normalizar(termo);

        if (busca.Length == 0)
            return true;

        if (Normalizar(nome).Contains(busca))
            return true;

        string digitosBusca = CpfValidador.SomenteDigitos(termo);

        return digitosBusca.Length >= 2
            && CpfValidador.SomenteDigitos(telefone).Contains(digitosBusca);
    }
}