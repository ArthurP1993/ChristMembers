using System.Globalization;

namespace ChristMembers.Models;

public class Aniversariante
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public string Nome { get; init; } = string.Empty;

    public string Telefone { get; init; } = string.Empty;

    public DateTime Data { get; init; }

    public int Dias { get; init; }

    public int Idade { get; init; }

    public bool EhHoje => Dias == 0;

    public bool PodeWhatsApp => Telefone.Length >= 10;

    public string PrimeiroNome =>
        Nome.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? Nome;

    public string TextoQuando => Dias switch
    {
        0 => "Hoje 🎉",
        1 => "Amanhã",
        > 1 => $"Em {Dias} dias",
        -1 => "Ontem",
        _ => "Já passou"
    };

    public string TextoIdade
    {
        get
        {
            string anos = Idade == 1 ? "1 ano" : $"{Idade} anos";

            return Dias >= 0 ? $"Faz {anos}" : $"Fez {anos}";
        }
    }

    public string TextoData => Data.ToString("dddd, dd/MM", PtBr);
}