using SQLite;

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

public class PedidoOracao
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public bool Anonimo { get; set; }

    public string Telefone { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public string Texto { get; set; } = string.Empty;

    public bool PermiteMural { get; set; }

    public StatusPedido Status { get; set; } = StatusPedido.Novo;

    public string Anotacao { get; set; } = string.Empty;

    public DateTime DataPedido { get; set; }

    public bool Arquivado { get; set; }

    public int Oracoes { get; set; }

    public bool AprovadoMural { get; set; }

    [Ignore]
    public string NomeExibicao =>
        Anonimo || string.IsNullOrWhiteSpace(Nome) ? "Anônimo" : Nome;

    [Ignore]
    public bool TemTelefone => !string.IsNullOrWhiteSpace(Telefone);

    [Ignore]
    public string TextoStatus => Status switch
    {
        StatusPedido.Novo => "🆕 Novo",
        StatusPedido.EmOracao => "🙏 Em oração",
        _ => "✅ Respondido"
    };

    [Ignore]
    public string TextoProximaAcao => Status switch
    {
        StatusPedido.Novo => "Marcar em oração",
        StatusPedido.EmOracao => "Marcar como respondido",
        _ => "Reabrir pedido"
    };

    [Ignore]
    public StatusPedido ProximoStatus => Status switch
    {
        StatusPedido.Novo => StatusPedido.EmOracao,
        StatusPedido.EmOracao => StatusPedido.Respondido,
        _ => StatusPedido.Novo
    };

    [Ignore]
    public string DataTexto => DataPedido.ToString("dd/MM/yyyy");

    [Ignore]
    public bool JaOrei { get; set; }

    [Ignore]
    public bool PodeOrar => !JaOrei;

    [Ignore]
    public string TextoBotaoOrar => JaOrei ? "Você está orando ✓" : "Estou orando 🙏";

    [Ignore]
    public string TextoOracoes => Oracoes switch
    {
        0 => "Seja a primeira pessoa a orar",
        1 => "1 pessoa orando",
        _ => $"{Oracoes} pessoas orando"
    };

    [Ignore]
    public string TextoAcaoMural => AprovadoMural ? "Retirar do mural" : "Publicar no mural";

    [Ignore]
    public string TextoSituacaoMural => AprovadoMural
        ? "🌐 Publicado no mural"
        : "🌐 Autorizou o mural (aguardando sua aprovação)";
}