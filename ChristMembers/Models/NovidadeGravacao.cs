using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

// Só as colunas da tabela. Usada apenas para gravar.
[Table("tb_Novidade")]
public class NovidadeGravacao : BaseModel
{
    [PrimaryKey("id_Novidade", false)]
    public long Id { get; set; }

    [Column("ds_Titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("ds_Descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("ds_NomeFoto")]
    public string NomeArquivoFoto { get; set; } = string.Empty;

    [Column("dt_DataEvento")]
    public DateTime? DataEvento { get; set; }
}