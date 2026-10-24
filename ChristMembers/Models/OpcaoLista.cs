namespace ChristMembers.Models;

public class OpcaoLista
{
    public long? Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public static OpcaoLista Nenhuma() => new() { Id = null, Nome = "(nenhum)" };
}