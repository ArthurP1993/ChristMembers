using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_ListaEstadoCivil")]
public class EstadoCivil : BaseModel
{
    [PrimaryKey("id_EstadoCivil", false)]
    public long Id { get; set; }

    [Column("ds_EstadoCivil")]
    public string Descricao { get; set; } = string.Empty;
}