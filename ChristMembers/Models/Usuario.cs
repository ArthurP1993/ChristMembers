using SQLite;

namespace ChristMembers.Models;

public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique]
    public string NomeUsuario { get; set; } = string.Empty;

    public string Sal { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public string Cargo { get; set; } = "Administrador";

    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; }
}