using SQLite;

namespace ChristMembers.Models;

public class Membro
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; }

    public string Telefone { get; set; }
}