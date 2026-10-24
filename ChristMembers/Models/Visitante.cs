using ChristMembers.Helpers;
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_Visitante")]
public class Visitante : BaseModel
{
    [PrimaryKey("id_Visitante", false)]
    public long Id { get; set; }

    [Column("ds_Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("ds_Telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("dt_DataVisita", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime DataVisita { get; set; }

    [Column("fl_Ativo")]
    public bool Ativo { get; set; } = true;

    [Column("id_Status")]
    public int IdStatus { get; set; }

    [Column("ds_Anotacao")]
    public string Anotacao { get; set; } = string.Empty;

    [Column("dt_Contato", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime? DataContato { get; set; }

    // ---------- Apenas para a tela (não são colunas) ----------

    [JsonIgnore]
    public StatusVisitante Status
    {
        get => (StatusVisitante)IdStatus;
        set => IdStatus = (int)value;
    }

    [JsonIgnore]
    public string TextoStatus => Status switch
    {
        StatusVisitante.Novo => "🆕 Novo",
        StatusVisitante.Contatado => "📞 Contatado",
        _ => "🙌 Voltou"
    };

    [JsonIgnore]
    public string TextoProximaAcao => Status switch
    {
        StatusVisitante.Novo => "Marcar como contatado",
        StatusVisitante.Contatado => "Marcar como voltou",
        _ => "Reabrir (novo)"
    };

    [JsonIgnore]
    public StatusVisitante ProximoStatus => Status switch
    {
        StatusVisitante.Novo => StatusVisitante.Contatado,
        StatusVisitante.Contatado => StatusVisitante.Voltou,
        _ => StatusVisitante.Novo
    };

    [JsonIgnore]
    public bool TemContato => DataContato.HasValue;

    [JsonIgnore]
    public bool PodeWhatsApp => CpfValidador.SomenteDigitos(Telefone).Length >= 10;

    [JsonIgnore]
    public string PrimeiroNome =>
        Nome.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? Nome;
}