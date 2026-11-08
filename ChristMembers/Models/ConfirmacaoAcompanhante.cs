using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_ConfirmacaoAcompanhante")]
public class ConfirmacaoAcompanhante : BaseModel
{
    [PrimaryKey("id_Acompanhante", false)]
    public long Id { get; set; }

    [Column("id_Confirmacao")]
    public long IdConfirmacao { get; set; }

    [Column("ds_Nome")]
    public string Nome { get; set; } = string.Empty;
}