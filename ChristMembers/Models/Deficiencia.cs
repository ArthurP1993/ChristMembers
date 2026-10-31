using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_ListaDeficiencia")]
public class Deficiencia : BaseModel
{
    [PrimaryKey("id_Deficiencia", false)]
    public long Id { get; set; }

    [Column("ds_Deficiencia")]
    public string Descricao { get; set; } = string.Empty;

    [Column("fl_Outros")]
    public bool Outros { get; set; }
}