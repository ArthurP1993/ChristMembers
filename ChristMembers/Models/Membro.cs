using SQLite;

namespace ChristMembers.Models;

public class Membro
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string CPF { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Email { get; set; }
    public string CEP { get; set; }
    public string Endereco { get; set; }
    public string Numero { get; set; }
    public string Complemento { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public DateTime? DataEntrada { get; set; }
    public bool? FoiBatizado { get; set; }
    public string Observacao { get; set; }
}