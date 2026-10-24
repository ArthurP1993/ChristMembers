using System.Globalization;
using ChristMembers.Config;
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

[Table("tb_Novidade")]
public class Novidade : BaseModel
{
    [PrimaryKey("id_Novidade", false)]
    public long Id { get; set; }

    [Column("ds_Titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("ds_Descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("ds_NomeFoto")]
    public string NomeArquivoFoto { get; set; } = string.Empty;

    [Column("dt_DataPublicacao", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime DataPublicacao { get; set; }

    [Column("dt_DataEvento")]
    public DateTime? DataEvento { get; set; }

    // ---------- Apenas para a tela (não são colunas) ----------

    [JsonIgnore]
    public bool TemFoto => !string.IsNullOrEmpty(NomeArquivoFoto);

    [JsonIgnore]
    public string CaminhoFoto => TemFoto
        ? $"{SupabaseConfig.Url.TrimEnd('/')}/storage/v1/object/public/novidades/{NomeArquivoFoto}"
        : string.Empty;

    [JsonIgnore]
    public bool Expirada =>
        DataEvento.HasValue && DataEvento.Value.Date < DateTime.Today;

    [JsonIgnore]
    public bool TemDataEvento => DataEvento.HasValue;

    [JsonIgnore]
    public string TextoEvento
    {
        get
        {
            if (!DataEvento.HasValue)
                return string.Empty;

            DateTime dia = DataEvento.Value.Date;

            if (dia == DateTime.Today)
                return "Hoje";

            if (dia == DateTime.Today.AddDays(1))
                return "Amanhã";

            return dia.ToString("dddd, dd/MM", new CultureInfo("pt-BR"));
        }
    }
}