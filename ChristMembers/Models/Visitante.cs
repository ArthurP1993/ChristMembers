using SQLite;

namespace ChristMembers.Models;

public class Visitante
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public DateTime DataVisita { get; set; }

    public bool Inativo { get; set; }
}