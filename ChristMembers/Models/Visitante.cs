using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_Visitante")]
public class Visitante : BaseModel
{
    [PrimaryKey("id_Visitante", false)]
    public long Id { get; set; }

    [Column("ds_Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("ds_Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("dt_DataVisita", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime DataVisita { get; set; }

    [Column("fl_Ativo")]
    public bool Ativo { get; set; } = true;
}