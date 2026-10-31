using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_ListaGenero")]
public class Genero : BaseModel
{
    [PrimaryKey("id_Genero", false)]
    public long Id { get; set; }

    [Column("ds_Genero")]
    public string Descricao { get; set; } = string.Empty;
}