using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_ListaConsagracao")]
public class Consagracao : BaseModel
{
    [PrimaryKey("id_Consagracao", false)]
    public int Id { get; set; }

    [Column("ds_Consagracao")]
    public string Descricao { get; set; } = string.Empty;
}