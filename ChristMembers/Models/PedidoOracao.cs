using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChristMembers.Models;

public static class CategoriasOracao
{
    public static readonly List<string> Todas = new()
    {
        "Saúde",
        "Família",
        "Trabalho e finanças",
        "Gratidão",
        "Outro"
    };
}

[Table("tb_PedidoOracao")]
public class PedidoOracao : BaseModel
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

    [Column("dt_Pedido", ignoreOnInsert: true, ignoreOnUpdate: true)]
    public DateTime DataPedido { get; set; }

    [Column("fl_Arquivado")]
    public bool Arquivado { get; set; }

    [Column("nu_Oracoes", ignoreOnUpdate: true)]
    public int Oracoes { get; set; }

    [Column("fl_AprovadoMural")]
    public bool AprovadoMural { get; set; }

    // ---------- Apenas para a tela (não são colunas) ----------

    [JsonIgnore]
    public string Categoria { get; set; } = string.Empty;

    [JsonIgnore]
    public StatusPedido Status
    {
        get => (StatusPedido)IdStatus;
        set => IdStatus = (int)value;
    }

    [JsonIgnore]
    public bool JaOrei { get; set; }

    [JsonIgnore]
    public string NomeExibicao =>
        Anonimo || string.IsNullOrWhiteSpace(Nome) ? "Anônimo" : Nome;

    [JsonIgnore]
    public bool TemTelefone => !string.IsNullOrWhiteSpace(Telefone);

    [JsonIgnore]
    public string DataTexto => DataPedido.ToString("dd/MM/yyyy");

    [JsonIgnore]
    public string TextoStatus => Status switch
    {
        StatusPedido.Novo => "🆕 Novo",
        StatusPedido.EmOracao => "🙏 Em oração",
        _ => "✅ Respondido"
    };

    [JsonIgnore]
    public string TextoProximaAcao => Status switch
    {
        StatusPedido.Novo => "Marcar em oração",
        StatusPedido.EmOracao => "Marcar como respondido",
        _ => "Reabrir pedido"
    };

    [JsonIgnore]
    public StatusPedido ProximoStatus => Status switch
    {
        StatusPedido.Novo => StatusPedido.EmOracao,
        StatusPedido.EmOracao => StatusPedido.Respondido,
        _ => StatusPedido.Novo
    };

    [JsonIgnore]
    public bool PodeOrar => !JaOrei;

    [JsonIgnore]
    public string TextoBotaoOrar => JaOrei ? "Você está orando ✓" : "Estou orando 🙏";

    [JsonIgnore]
    public string TextoOracoes => Oracoes switch
    {
        0 => "Seja a primeira pessoa a orar",
        1 => "1 pessoa orando",
        _ => $"{Oracoes} pessoas orando"
    };

    [JsonIgnore]
    public string TextoAcaoMural => AprovadoMural ? "Retirar do mural" : "Publicar no mural";

    [JsonIgnore]
    public string TextoSituacaoMural => AprovadoMural
        ? "🌐 Publicado no mural"
        : "🌐 Autorizou o mural (aguardando sua aprovação)";
}