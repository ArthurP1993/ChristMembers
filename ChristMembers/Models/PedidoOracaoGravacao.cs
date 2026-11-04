using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

// Só as colunas da tabela. Usada apenas para gravar.
[Table("tb_PedidoOracao")]
public class PedidoOracaoGravacao : BaseModel
{
    [PrimaryKey("id_PedidoOracao", false)]
    public long Id { get; set; }

    [Column("ds_Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("fl_Anonimo")]
    public bool Anonimo { get; set; }

    [Column("ds_Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("id_Categoria")]
    public int IdCategoria { get; set; }

    [Column("ds_Texto")]
    public string Texto { get; set; } = string.Empty;

    [Column("fl_PermiteMural")]
    public bool PermiteMural { get; set; }

    [Column("id_Status")]
    public int IdStatus { get; set; }

    [Column("ds_Anotacao")]
    public string Anotacao { get; set; } = string.Empty;

    [Column("fl_Arquivado")]
    public bool Arquivado { get; set; }

    // O contador de orações nunca é alterado por uma edição.
    [Column("nu_Oracoes", ignoreOnUpdate: true)]
    public int Oracoes { get; set; }

    [Column("fl_AprovadoMural")]
    public bool AprovadoMural { get; set; }

    public static PedidoOracaoGravacao De(PedidoOracao p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Anonimo = p.Anonimo,
        Telefone = p.Telefone,
        IdCategoria = p.IdCategoria,
        Texto = p.Texto,
        PermiteMural = p.PermiteMural,
        IdStatus = p.IdStatus,
        Anotacao = p.Anotacao,
        Arquivado = p.Arquivado,
        Oracoes = p.Oracoes,
        AprovadoMural = p.AprovadoMural
    };
}