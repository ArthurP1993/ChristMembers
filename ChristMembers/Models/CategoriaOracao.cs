using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_ListaCategoriaOracao")]
public class CategoriaOracao : BaseModel
{
    [PrimaryKey("id_Categoria", false)]
    public int Id { get; set; }

    [Column("ds_Categoria")]
    public string Descricao { get; set; } = string.Empty;
}