using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_Membros")]
public class Membro : BaseModel
{
    [PrimaryKey("id_Membro", false)]
    public long Id { get; set; }

    [Column("ds_Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("ds_Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("ds_CPF")]
    public string CPF { get; set; } = string.Empty;

    [Column("dt_DataNascimento")]
    public DateTime? DataNascimento { get; set; }

    [Column("ds_Email")]
    public string Email { get; set; } = string.Empty;

    [Column("ds_CEP")]
    public string CEP { get; set; } = string.Empty;

    [Column("ds_Endereco")]
    public string Endereco { get; set; } = string.Empty;

    [Column("ds_Numero")]
    public string Numero { get; set; } = string.Empty;

    [Column("ds_Complemento")]
    public string Complemento { get; set; } = string.Empty;

    [Column("ds_Bairro")]
    public string Bairro { get; set; } = string.Empty;

    [Column("ds_Cidade")]
    public string Cidade { get; set; } = string.Empty;

    [Column("ds_Estado")]
    public string Estado { get; set; } = string.Empty;

    [Column("dt_DataEntrada")]
    public DateTime? DataEntrada { get; set; }

    [Column("fl_FoiBatizado")]
    public bool? FoiBatizado { get; set; }

    [Column("dt_Batismo")]
    public DateTime? DataBatismo { get; set; }

    [Column("ds_Observacao")]
    public string Observacao { get; set; } = string.Empty;

    [Column("fl_Ativo")]
    public bool Ativo { get; set; } = true;

    [Column("id_Auth")]
    public string? IdAuth { get; set; }

    [Column("fl_Consentimento")]
    public bool Consentimento { get; set; }

    [Column("dt_Consentimento", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime? DataConsentimento { get; set; }

    [Column("id_Consagracao")]
    public long? IdConsagracao { get; set; }

    [Column("id_Setor")]
    public long? IdSetor { get; set; }

    [Column("id_EstadoCivil")]
    public long? IdEstadoCivil { get; set; }

    [Column("fl_TemFilhos")]
    public bool? TemFilhos { get; set; }

    [Column("id_Genero")]
    public long? IdGenero { get; set; }

    [Column("fl_PossuiDeficiencia")]
    public bool? PossuiDeficiencia { get; set; }

    [Column("id_Deficiencia")]
    public long? IdDeficiencia { get; set; }

    [Column("ds_DeficienciaOutros")]
    public string? DeficienciaOutros { get; set; }

    [Column("fl_ConsentimentoSaude")]
    public bool ConsentimentoSaude { get; set; }

    // ---------- Apenas para a tela (NÃO são colunas) ----------

    [Newtonsoft.Json.JsonIgnore]
    public string NomeConsagracao { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonIgnore]
    public string NomeSetor { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonIgnore]
    public string NomeEstadoCivil { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonIgnore]
    public string NomeGenero { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonIgnore]
    public string NomeDeficiencia { get; set; } = string.Empty;

    [Column("dt_Cadastro", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime? DataCadastro { get; set; }

    [Column("ds_SiglaCongregacao")]
    public string? SiglaCongregacao { get; set; }

    [Column("ds_NomeFoto", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public string? NomeFoto { get; set; }
}