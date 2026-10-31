namespace ChristMembers.Models;

public class OpcaoLista
{
    public long? Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    // Usada na lista de deficiências: abre a caixa de texto livre.
    public bool Outros { get; set; }

    public static OpcaoLista Nenhuma() => new() { Id = null, Nome = "(nenhum)" };
}