using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_Usuario")]
public class PerfilUsuario : BaseModel
{
    [PrimaryKey("id_Usuario", false)]
    public long Id { get; set; }

    [Column("ds_Nome")]
    public string? Nome { get; set; }

    [Column("ds_Email")]
    public string? Email { get; set; }

    [Column("id_Auth")]
    public string? IdAuth { get; set; }

    [Column("id_Setor")]
    public long? IdSetor { get; set; }

    [Column("id_Consagracao")]
    public long? IdConsagracao { get; set; }

    [Column("fl_Ativo")]
    public bool Ativo { get; set; }

    [JsonIgnore]
    public string Cargo { get; set; } = string.Empty;
}