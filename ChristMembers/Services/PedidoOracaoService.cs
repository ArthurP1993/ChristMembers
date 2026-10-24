using System.Collections.ObjectModel;
using ChristMembers.Models;
using Newtonsoft.Json;
using Supabase.Postgrest;

namespace ChristMembers.Services;

public class PedidoOracaoService
{
    private readonly SupabaseServico _supabase;

    private List<CategoriaOracao>? _categorias;

    public ObservableCollection<PedidoOracao> Pedidos { get; } = new();

    public PedidoOracaoService(SupabaseServico supabase)
    {
        _supabase = supabase;
    }

    // ---------- Categorias ----------

    private async Task<List<CategoriaOracao>> ObterCategorias()
    {
        if (_categorias != null && _categorias.Count > 0)
            return _categorias;

        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.From<CategoriaOracao>().Get();

        _categorias = resposta.Models;

        return _categorias;
    }

    private static string NomeDaCategoria(int id, List<CategoriaOracao> categorias)
    {
        return categorias.FirstOrDefault(c => c.Id == id)?.Descricao ?? "Outro";
    }

    // ---------- Admin ----------

    public async Task Inicializar()
    {
        var cliente = await _supabase.ObterAsync();

        List<CategoriaOracao> categorias = await ObterCategorias();

        var resposta = await cliente
            .From<PedidoOracao>()
            .Where(p => p.Arquivado == false)
            .Get();

        Pedidos.Clear();

        foreach (PedidoOracao pedido in resposta.Models.OrderByDescending(p => p.Id))
        {
            pedido.Categoria = NomeDaCategoria(pedido.IdCategoria, categorias);

            Pedidos.Add(pedido);
        }
    }

    public async Task<int> ContarNovos()
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente
            .From<PedidoOracao>()
            .Where(p => p.IdStatus == 0)
            .Where(p => p.Arquivado == false)
            .Get();

        return resposta.Models.Count;
    }

    public async Task AlterarStatus(PedidoOracao pedido, StatusPedido status)
    {
        StatusPedido anterior = pedido.Status;

        pedido.Status = status;

        await Salvar(pedido, () => pedido.Status = anterior);

        AtualizarNaLista(pedido);
    }

    public async Task SalvarAnotacao(PedidoOracao pedido, string anotacao)
    {
        string anterior = pedido.Anotacao;

        pedido.Anotacao = anotacao.Trim();

        await Salvar(pedido, () => pedido.Anotacao = anterior);

        AtualizarNaLista(pedido);
    }

    public async Task AlternarMural(PedidoOracao pedido)
    {
        bool anterior = pedido.AprovadoMural;

        pedido.AprovadoMural = !anterior;

        await Salvar(pedido, () => pedido.AprovadoMural = anterior);

        AtualizarNaLista(pedido);
    }

    public async Task Arquivar(PedidoOracao pedido)
    {
        pedido.Arquivado = true;

        await Salvar(pedido, () => pedido.Arquivado = false);

        Pedidos.Remove(pedido);
    }

    private async Task Salvar(PedidoOracao pedido, Action reverter)
    {
        var cliente = await _supabase.ObterAsync();

        try
        {
            await cliente.From<PedidoOracao>().Update(pedido);
        }
        catch
        {
            reverter();
            throw;
        }
    }

    private void AtualizarNaLista(PedidoOracao pedido)
    {
        int indice = Pedidos.IndexOf(pedido);

        if (indice >= 0)
            Pedidos[indice] = pedido;
    }

    // ---------- Qualquer pessoa ----------

    public async Task Enviar(
        string nome,
        bool anonimo,
        string telefone,
        string categoria,
        string texto,
        bool permiteMural)
    {
        var cliente = await _supabase.ObterAsync();

        List<CategoriaOracao> categorias = await ObterCategorias();

        CategoriaOracao? escolhida = categorias.FirstOrDefault(c => c.Descricao == categoria);

        if (escolhida == null)
            throw new InvalidOperationException("Categoria não encontrada no banco.");

        PedidoOracao pedido = new()
        {
            Nome = anonimo ? string.Empty : nome.Trim(),
            Anonimo = anonimo,
            Telefone = anonimo ? string.Empty : telefone.Trim(),
            IdCategoria = escolhida.Id,
            Texto = texto.Trim(),
            PermiteMural = permiteMural,
            IdStatus = 0
        };

        await cliente
            .From<PedidoOracao>()
            .Insert(pedido, new QueryOptions { Returning = QueryOptions.ReturnType.Minimal });
    }

    // ---------- Mural ----------

    public async Task<List<PedidoOracao>> ListarMural()
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.Rpc("listar_mural", null);

        List<ItemMural> itens =
            JsonConvert.DeserializeObject<List<ItemMural>>(resposta.Content ?? "[]")
            ?? new List<ItemMural>();

        return itens
            .Select(i => new PedidoOracao
            {
                Id = i.Id,
                Categoria = i.Categoria,
                Texto = i.Texto,
                DataPedido = i.Data,
                Oracoes = i.Oracoes
            })
            .ToList();
    }

    public async Task<int> RegistrarOracao(long id)
    {
        var cliente = await _supabase.ObterAsync();

        var resposta = await cliente.Rpc(
            "orar",
            new Dictionary<string, object> { { "p_id", id } });

        return int.TryParse(resposta.Content?.Trim(), out int total) ? total : 0;
    }

    private class ItemMural
    {
        [JsonProperty("id_PedidoOracao")]
        public long Id { get; set; }

        [JsonProperty("ds_Categoria")]
        public string Categoria { get; set; } = string.Empty;

        [JsonProperty("ds_Texto")]
        public string Texto { get; set; } = string.Empty;

        [JsonProperty("dt_Pedido")]
        public DateTime Data { get; set; }

        [JsonProperty("nu_Oracoes")]
        public int Oracoes { get; set; }
    }
}