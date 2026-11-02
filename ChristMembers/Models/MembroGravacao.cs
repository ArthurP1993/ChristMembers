using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

// Só as colunas da tabela. Usada apenas para gravar.
[Table("tb_Membros")]
public class MembroGravacao : BaseModel
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

    // O vínculo com a conta e o aceite geral nunca mudam numa edição.
    [Column("id_Auth", ignoreOnUpdate: true)]
    public string? IdAuth { get; set; }

    [Column("fl_Consentimento", ignoreOnUpdate: true)]
    public bool Consentimento { get; set; }

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

    [Column("ds_SiglaCongregacao")]
    public string? SiglaCongregacao { get; set; }

    public static MembroGravacao De(Membro m) => new()
    {
        Id = m.Id,
        Nome = m.Nome,
        Telefone = m.Telefone,
        CPF = m.CPF,
        DataNascimento = m.DataNascimento,
        Email = m.Email,
        CEP = m.CEP,
        Endereco = m.Endereco,
        Numero = m.Numero,
        Complemento = m.Complemento,
        Bairro = m.Bairro,
        Cidade = m.Cidade,
        Estado = m.Estado,
        DataEntrada = m.DataEntrada,
        FoiBatizado = m.FoiBatizado,
        DataBatismo = m.DataBatismo,
        Observacao = m.Observacao,
        Ativo = m.Ativo,
        IdAuth = m.IdAuth,
        Consentimento = m.Consentimento,
        IdConsagracao = m.IdConsagracao,
        IdSetor = m.IdSetor,
        IdEstadoCivil = m.IdEstadoCivil,
        TemFilhos = m.TemFilhos,
        IdGenero = m.IdGenero,
        PossuiDeficiencia = m.PossuiDeficiencia,
        IdDeficiencia = m.IdDeficiencia,
        DeficienciaOutros = m.DeficienciaOutros,
        ConsentimentoSaude = m.ConsentimentoSaude,
        SiglaCongregacao = m.SiglaCongregacao
    };
}