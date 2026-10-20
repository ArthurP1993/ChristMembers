using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_ListaSetores")]
public class Setor : BaseModel
{
    [PrimaryKey("id_Setor", false)]
    public long Id { get; set; }

    [Column("ds_Setor")]
    public string Descricao { get; set; } = string.Empty;

    [Column("fl_Admin")]
    public bool Admin { get; set; }
}