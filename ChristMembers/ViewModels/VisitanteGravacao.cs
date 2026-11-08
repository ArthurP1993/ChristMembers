using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

// Só as colunas da tabela. Usada apenas para gravar.
[Table("tb_Visitante")]
public class VisitanteGravacao : BaseModel
{
    [PrimaryKey("id_Visitante", false)]
    public long Id { get; set; }

    [Column("ds_Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("ds_Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("fl_Ativo")]
    public bool Ativo { get; set; } = true;

    [Column("id_Status")]
    public int IdStatus { get; set; }

    [Column("ds_Anotacao")]
    public string Anotacao { get; set; } = string.Empty;

    // A data da visita e a data do contato ficam por conta do banco.

    public static VisitanteGravacao De(Visitante v) => new()
    {
        Id = v.Id,
        Nome = v.Nome,
        Telefone = v.Telefone,
        Ativo = v.Ativo,
        IdStatus = v.IdStatus,
        Anotacao = v.Anotacao ?? string.Empty
    };
}