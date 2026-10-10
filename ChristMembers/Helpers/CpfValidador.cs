namespace ChristMembers.Helpers;

public static class CpfValidador
{
    public static string SomenteDigitos(string? texto)
    {
        return new string((texto ?? string.Empty)
            .Where(char.IsDigit)
            .ToArray());
    }

    public static bool EhValido(string? cpf)
    {
        string digitos = SomenteDigitos(cpf);

        if (digitos.Length != 11)
            return false;

        // 111.111.111-11, 222.222.222-22 etc. não são válidos
        if (digitos.Distinct().Count() == 1)
            return false;

        int primeiro = CalcularDigito(digitos, 9);
        int segundo = CalcularDigito(digitos, 10);

        return digitos[9] - '0' == primeiro
            && digitos[10] - '0' == segundo;
    }

    private static int CalcularDigito(string digitos, int quantidade)
    {
        int soma = 0;

        for (int i = 0; i < quantidade; i++)
            soma += (digitos[i] - '0') * (quantidade + 1 - i);

        int resto = soma % 11;

        return resto < 2 ? 0 : 11 - resto;
    }
}