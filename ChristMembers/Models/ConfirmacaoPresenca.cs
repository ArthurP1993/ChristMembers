using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_ConfirmacaoPresenca")]
public class ConfirmacaoPresenca : BaseModel
{
    [PrimaryKey("id_Confirmacao", false)]
    public long Id { get; set; }

    [Column("id_Novidade")]
    public long IdNovidade { get; set; }

    [Column("id_Auth")]
    public string? IdAuth { get; set; }

    [Column("ds_Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("ds_Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("fl_Membro", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public bool EhMembro { get; set; }

    [Column("dt_Cadastro", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime DataCadastro { get; set; }

    // Chave secreta da confirmação. Só o dono e o administrador conseguem ler.
    [Column("cd_Token", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public string? Token { get; set; }
}